using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace WcfWeatherService
{
    class OpenWeatherMapService
    {
        private const string EndPoint = "http://api.openweathermap.org/data/2.5/weather?";
        public readonly string ApiKey;
        public OpenWeatherMapService(string apiKey)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        /// Obtaining weather data from OpenWeatherMap service by city name and country code
        /// </summary>
        /// <param name="city"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public OpenWeather GetDataByCityName(string city, string countryCode)
        {
            try
            {
                string url = EndPoint + "q=" + city + "," + countryCode + "&APPID=" + ApiKey + "&units=metric";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(
                            string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    Stream stream = response.GetResponseStream();
                    string json = "";
                    if (stream != null)
                    {
                        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                        json = sr.ReadToEnd();
                    }

                    OpenWeather obj = JsonConvert.DeserializeObject<OpenWeather>(json);
                    return obj;
                }
            }
            catch (Exception e)
            {
                Output.ShowError(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtaining weather data from OpenWeatherMap service by city id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public OpenWeather GetDataByCityId(int cityId)
        {
            try
            {
                string url = EndPoint + "id=" + cityId + "&APPID=" + ApiKey + "&units=metric";
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(
                            string.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    Stream stream = response.GetResponseStream();
                    string json = "";
                    if (stream != null)
                    {
                        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                        json = sr.ReadToEnd();
                    }

                    OpenWeather obj = JsonConvert.DeserializeObject<OpenWeather>(json);
                    return obj;
                }
            }
            catch (Exception e)
            {
                Output.ShowError(e.Message);
                return null;
            }
        }
    }
}
