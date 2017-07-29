using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace WcfWeatherService
{
    /// <summary>
    /// The functionality of the service
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IWeatherCallback))]
    public interface IWeather
    {
        /// <summary>
        /// Logging to the service and getting a client ID
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>Return userID</returns>
        [OperationContract]
        [FaultContract(typeof(NotCorrectLoginOrPassword))]
        string LogInToService(string login, string password);
        /// <summary>
        /// Find city id by name
        /// </summary>
        /// <param name="name">The name of city</param>
        /// <param name="countryCode">Country code accordingly to ISO 3166-2. You can specify as null</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(CityNotFound))]
        int GetCityIdByName(string name, string countryCode = null);
        /// <summary>
        /// Subscribe to receive updates of weather forecasts by city name
        /// </summary>
        /// <param name="uniqueId">User id</param>
        /// <param name="city">The name of city</param>
        /// <param name="country">The country name or country code accordingly to ISO 3166-2</param>
        [OperationContract]
        [FaultContract(typeof(CityNotFound))]
        [FaultContract(typeof(UniqueClientIdNotFound))]
        [FaultContract(typeof(ServerHasNoWeatherData))]
        [FaultContract(typeof(CountryNotFound))]
        void RegisterForUpdatesByCityName(string uniqueId, string city, string country);
        /// <summary>
        /// Subscribe to receive updates of weather forecasts by city id
        /// </summary>
        /// <param name="uniqueId">User id</param>
        /// <param name="cityId">The city id. To get the correct id, use the function GetCityIdByName</param>
        [OperationContract]
        [FaultContract(typeof(CityNotFound))]
        [FaultContract(typeof(UniqueClientIdNotFound))]
        [FaultContract(typeof(ServerHasNoWeatherData))]
        void RegisterForUpdatesByCityId(string uniqueId, int cityId);
    }

    /// <summary>
    /// Interface to send weather via Callback
    /// </summary>
    public interface IWeatherCallback
    {
        /// <summary>
        /// Sending the weather to the user via Callback
        /// </summary>
        /// <param name="weather">Current weather forecast</param>
        [OperationContract(IsOneWay = true)]
        void WeatherUpdate(OpenWeather weather);
    }

    /// <summary>
    /// Implementation of the service
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.PerCall)]
    public class WeatherService : IWeather
    {
        //Your OpenWeather ApiKey
        private const string OWAPIKEY = "aui437fe03963ebd9cf2333292930746";
        private static List<City> cities;
        public WeatherService()
        {
            //Initialize list of cities served by the OpenWeatherMap service
            using (StreamReader r = new StreamReader($"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\AppData\\city.list.json"))
                cities = JsonConvert.DeserializeObject<List<City>>(r.ReadToEnd());
        }
        public class CallbacksRequest
        {
            public ForCity forCity;
            public List<IWeatherCallback> callbackProcesses;

            public CallbacksRequest()
            {
                callbackProcesses = new List<IWeatherCallback>();
            }
        }
        public static Hashtable requests = new Hashtable();

        public string LogInToService(string login, string password)
        {
            using (AppContext db = new AppContext())
            {
                string uniqueId = db.Clients.Where(x => x.Login == login && x.Password == password).Select(x => x.ClientId).SingleOrDefault();
                if (uniqueId != null)
                    return uniqueId;
                else
                    throw new FaultException<NotCorrectLoginOrPassword>(new NotCorrectLoginOrPassword("Login or password is not correct. Please check the correctness of input data."));
            }
        }

        public int GetCityIdByName(string name, string countryCode = null)
        {
            City city = null;
            if (countryCode == null)
                city = cities.Where(x => x.Name == name).FirstOrDefault();
            else
                city = cities.Where(x => x.Name == name && x.CountryCode == countryCode).SingleOrDefault();

            if (city == null)
                if (countryCode == null)
                    throw new FaultException<CityNotFound>(new CityNotFound($"The city with name '{name}' not found!  Please check the city name."),
                        new FaultReason($"The city with the name '{name}' not found!"));
                else
                    throw new FaultException<CityNotFound>(new CityNotFound($"The city with the name '{name}' and country code '{countryCode}' not found! Please check the city name and country code."), 
                        new FaultReason($"The city with the name '{name}' and country code '{countryCode}' not found!"));
            return city.CityId;
        }

        public void RegisterForUpdatesByCityName(string uniqueId, string city, string country)
        {
            try
            {
                using (AppContext db = new AppContext())
                {
                    CheckUniqueId(uniqueId, db);

                    country = db.Countries
                        .Where(x => x.Code == country || x.Name == country)
                        .Select(x => x.Code)
                        .FirstOrDefault();

                    if(country == null)
                        throw new FaultException<CountryNotFound>(new CountryNotFound(), new FaultReason("Country name or code not found."));
                }

                GetCityIdByName(city, country);

                AddClientToReceiveWeather(new ForCity(city, country));
            }
            catch (FaultException<CityNotFound> e)
            {
                e.Detail.Message = $"{e.Message} Please check the city name and country code.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (FaultException<UniqueClientIdNotFound> e)
            {
                e.Detail.Message = $"{e.Message} To obtain the correct unique identifier you need to login to service and get it back.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (FaultException<CountryNotFound> e)
            {
                e.Detail.Message = $"{e.Message} Check the input data!";
                Output.ShowError(e.Message);
                throw e;
            }
            catch(FaultException<ServerHasNoWeatherData> e)
            {
                e.Detail.Message = $"{e.Message} Contact the administration to solve the problem.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (Exception e)
            {
                Output.ShowError(e.Message);
            }
        }

        public void RegisterForUpdatesByCityId(string uniqueId, int cityId)
        {
            try
            {
                CheckUniqueId(uniqueId);

                using (StreamReader r = new StreamReader($"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\AppData\\city.list.json"))
                {
                    List<City> cities = JsonConvert.DeserializeObject<List<City>>(r.ReadToEnd());

                    City city = cities.Where(x => x.CityId == cityId).SingleOrDefault();
                    if (city == null)
                        throw new FaultException<CityNotFound>(new CityNotFound(), new FaultReason($"The city with id '{cityId}' not found!"));

                    AddClientToReceiveWeather(new ForCity(city.Name, city.CountryCode));
                }
            }
            catch (FaultException<CityNotFound> e)
            {
                e.Detail.Message = $"{e.Message} To get the correct id, use the function GetCityIdByName.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (FaultException<UniqueClientIdNotFound> e)
            {
                e.Detail.Message = $"{e.Message} To obtain the correct unique identifier you need to login to service and get it back.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (FaultException<CountryNotFound> e)
            {
                e.Detail.Message = $"{e.Message} Check the input data!";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (FaultException<ServerHasNoWeatherData> e)
            {
                e.Detail.Message = $"{e.Message} Contact the administration to solve the problem.";
                Output.ShowError(e.Message);
                throw e;
            }
            catch (Exception e)
            {
                Output.ShowError(e.Message);
            }
        }

        /// <summary>
        /// Check whether the client with this uniqueId
        /// </summary>
        /// <param name="uniqueId">Client id</param>
        /// <param name="db">EF context</param>
        private void CheckUniqueId(string uniqueId, AppContext db = null)
        {
            if (db == null) db = new AppContext();
            string id = db.Clients.Where(x => x.ClientId == uniqueId)
                .Select(x => x.ClientId).SingleOrDefault();
            if (id == null)
                throw new FaultException<UniqueClientIdNotFound>(new UniqueClientIdNotFound(), new FaultReason("Not found the unique client ID."));
        }
        /// <summary>
        /// Adding a client to the newsletter the weather and the first sending the weather
        /// </summary>
        /// <param name="city"></param>
        private void AddClientToReceiveWeather(ForCity city)
        {
            CallbacksRequest req = null;
            if (!requests.ContainsKey(city))
            {
                Output.ShowInfo($"Not сontains {city.City}");
                req = new CallbacksRequest() { forCity = city };

                requests.Add(city, req);

                Thread t = new Thread(CheckUpdate);
                t.IsBackground = true;
                t.Start(city);
            }
            else Output.ShowInfo($"Contains {city.City}");

            req = (CallbacksRequest)requests[city];

            IWeatherCallback callback = OperationContext.Current.GetCallbackChannel<IWeatherCallback>();
            if (!req.callbackProcesses.Contains(callback))
            {
                lock (req.callbackProcesses) req.callbackProcesses.Add(callback);

                //Sending a forecast immediately after the subscription to it
                OpenWeatherMapService weatherService = new OpenWeatherMapService(OWAPIKEY);
                OpenWeather weather = weatherService.GetDataByCityName(city.City, city.CountryCode);
                if (weather == null)
                    throw new FaultException<ServerHasNoWeatherData>
                        (new ServerHasNoWeatherData(), new FaultReason("The server has not received weather data."));
                else callback.WeatherUpdate(weather);
            }
        }
        /// <summary>
        /// Check for changing weather data. In the event of changes, then sending new data to the client
        /// </summary>
        /// <param name="city"></param>
        private void CheckUpdate(object city)
        {
            ForCity forCity = city as ForCity;
            OpenWeatherMapService weatherService = new OpenWeatherMapService(OWAPIKEY);
            OpenWeather weather, weatherToCheck;

            while (true)
            {
                weather = weatherService.GetDataByCityName(forCity.City, forCity.CountryCode);

                if (weather != null)
                {
                    using (AppContext db = new AppContext())
                    {
                        weatherToCheck = db.Weather.Where(x => x.CityId == weather.CityId)
                            .Include(x => x.Forecast)
                            .Include(x => x.Wind)
                            .Include(x => x.Clouds)
                            .FirstOrDefault();

                        if (weatherToCheck == null)
                        {
                            db.Weather.Add(weather);
                            db.SaveChanges();
                        }
                        else if (weather.Equals(weatherToCheck))
                        {
                            Thread.Sleep(15000);
                            continue;
                        }
                        else
                        {
                            if (weatherToCheck.Wind != null && !weather.Wind.Equals(weatherToCheck.Wind))
                            {
                                weatherToCheck.Wind.Direction = weather.Wind.Direction;
                                weatherToCheck.Wind.Speed = weather.Wind.Speed;
                            }
                            else
                            {
                                weatherToCheck.Wind = weather.Wind;
                            }
                            if (weatherToCheck.Forecast != null && !weather.Forecast.Equals(weatherToCheck.Forecast))
                            {
                                weatherToCheck.Forecast.Temp = weather.Forecast.Temp;
                                weatherToCheck.Forecast.Humidity = weather.Forecast.Humidity;
                                weatherToCheck.Forecast.Pressure = weather.Forecast.Pressure;
                                weatherToCheck.Forecast.TempMax = weather.Forecast.TempMax;
                                weatherToCheck.Forecast.TempMin = weather.Forecast.TempMin;
                            }
                            else
                            {
                                weatherToCheck.Forecast = weather.Forecast;
                            }
                            if (weatherToCheck.Clouds != null && !weather.Clouds.Equals(weatherToCheck.Clouds))
                            {
                                weatherToCheck.Clouds.Cloudiness = weather.Clouds.Cloudiness;
                            }
                            else
                            {
                                weatherToCheck.Clouds = weather.Clouds;
                            }

                            db.Entry(weatherToCheck).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    var z = requests[forCity] as CallbacksRequest;

                    if (z.callbackProcesses.Count < 1)
                    {
                        lock (requests) requests.Remove(forCity);
                        if (Thread.CurrentThread.IsBackground)
                            Thread.CurrentThread.Abort();
                    }
                    else
                    {
                        List<Thread> threads = new List<Thread>();

                        lock (z.callbackProcesses)
                            foreach (IWeatherCallback item in z.callbackProcesses)
                                threads.Add(new Thread(new ThreadStart(() => SendUpdateToClient(item, weather, forCity))));

                        threads.ForEach(x => x.Start());
                    }
                }

                Thread.Sleep(15000);
            }
        }
        /// <summary>
        /// Sending forecast and remove the client if it does not respond
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="weather"></param>
        /// <param name="city"></param>
        private void SendUpdateToClient(IWeatherCallback callback, OpenWeather weather, ForCity city)
        {
            try
            {
                lock ((requests[city] as CallbacksRequest).callbackProcesses)
                    (requests[city] as CallbacksRequest).callbackProcesses.Remove(callback);

                callback.WeatherUpdate(weather);

                if (requests[city] == null)
                {
                    CallbacksRequest req = new CallbacksRequest() { forCity = city };
                    req.callbackProcesses.Add(callback);

                    lock (requests) requests.Add(city, req);

                    Thread t = new Thread(CheckUpdate);
                    t.IsBackground = true;
                    t.Start(city as object);
                }
                else
                {
                    lock ((requests[city] as CallbacksRequest).callbackProcesses)
                        (requests[city] as CallbacksRequest).callbackProcesses.Add(callback);
                }
            }
            catch (TimeoutException)
            {
                Output.ShowError($"TimeoutException for {city.City} city. The city has been removed.");
            }
            catch (Exception ex)
            {
                Output.ShowError($"Error when sending a message to the client: {ex.Message}");
            }
            finally
            {
                Thread.CurrentThread.Abort();
            }
        }
    }
}
