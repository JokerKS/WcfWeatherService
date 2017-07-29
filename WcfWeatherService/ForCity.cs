using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WcfWeatherService
{
    public class ForCity
    {
        public string City { get; private set; }
        public string CountryCode { get; private set; }

        public ForCity(string city, string countryCode)
        {
            City = city;
            CountryCode = countryCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is ForCity)
            {
                ForCity p = (ForCity)(obj);
                if (City == p.City && CountryCode == p.CountryCode)
                    return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return City.GetHashCode() ^ CountryCode.GetHashCode();
        }
    }
    /// <summary>
    /// To use ISO 3166
    /// </summary>
    class Country
    {
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Key]
        [StringLength(2, MinimumLength = 2)]
        public string Code { get; set; }
    }

    class City
    {
        [Key]
        [JsonProperty("id")]
        public int CityId { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [JsonProperty("name")]
        public string Name { get; set; }
        [StringLength(2, MinimumLength = 2)]
        [JsonProperty("country")]
        public string CountryCode { get; set; }
    }
}
