using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;

namespace WcfWeatherService
{
    class AppContext : DbContext
    {
        public DbSet<OpenWeather> Weather { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Client> Clients { get; set; }
        public AppContext() : base("WeatherDB")
        {
            Database.SetInitializer(new WeatherDbInitializer());
        }
    }

    /// <summary>
    /// Data initialization when the database is created
    /// </summary>
    class WeatherDbInitializer : CreateDatabaseIfNotExists<AppContext>
    {
        protected override void Seed(AppContext context)
        {
            //Adding a list of countries from json
            using (StreamReader r = new StreamReader($"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\AppData\\ISO3166-2.json"))
            {
                List<Country> countries = JsonConvert.DeserializeObject<List<Country>>(r.ReadToEnd());

                foreach (var country in countries)
                    context.Countries.Add(country);
            }
            //Add default clients
            context.Clients.Add(new Client() { Login = "admin", Password = "admin", ClientId = UniqueIdGenerator.GenerateUniqueId() });
            context.Clients.Add(new Client() { Login = "testing", Password = "testing", ClientId = UniqueIdGenerator.GenerateUniqueId() });
            context.Clients.Add(new Client() { Login = "guest", Password = "guest", ClientId = UniqueIdGenerator.GenerateUniqueId() });
            //Write data to database
            context.SaveChanges();
        }
    }
}
