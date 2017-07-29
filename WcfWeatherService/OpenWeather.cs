using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WcfWeatherService
{
    [DataContract]
    [Table("OpenWeather")]
    public class OpenWeather : IEquatable<OpenWeather>
    {
        [DataMember]
        [Key, Column(Order = 1)]
        [JsonProperty("name")]
        public string City { get; set; }

        [DataMember]
        [Key, Column(Order = 0)]
        [JsonProperty("id")]
        public int CityId { get; set; }

        //TODO
        /*
        [DataMember(Name = "Weather")]
        public List<Weather> weather { get; set; }
        */

        [DataMember]
        [Required]
        [JsonProperty("main")]
        public Forecast Forecast { get; set; }

        [DataMember]
        [Required]
        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [DataMember]
        [Required]
        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        public override bool Equals(object other)
        {
            //Если не проверить на null объект other, то other.GetType() может выбросить //NullReferenceException.
            if (other == null)
                return false;
            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as OpenWeather);
        }
        public override int GetHashCode()
        {
            return City.GetHashCode() ^ CityId.GetHashCode() ^ Forecast.GetHashCode() 
                ^ Clouds.GetHashCode() ^ Wind.GetHashCode() /*^ weather.GetHashCode()*/;
        }

        public bool Equals(OpenWeather other)
        {
            if (other == null)
                return false;
            if (string.Compare(City, other.City) == 0 && CityId.Equals(other.CityId) && Forecast.Equals(other.Forecast) &&
                Clouds.Equals(other.Clouds) && Wind.Equals(other.Wind) /*&& weather.Equals(other.weather)*/)
                return true;
            else
                return false;
        }
    }
    [DataContract]
    public class Weather : IEquatable<Weather>
    {
        [Key]
        public int WeatherId { get; set; }
        [DataMember(Name = "GroupParameter")]
        [JsonProperty("main")]
        public string main { get; set; }
        [DataMember(Name = "GroupDescription")]
        [JsonProperty("description")]
        public string description { get; set; }
        [DataMember(Name = "WeatherIcon")]
        [JsonProperty("icon")]
        public string icon { get; set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as Weather);
        }
        public override int GetHashCode()
        {
            return main.GetHashCode() ^ description.GetHashCode() ^ icon.GetHashCode();
        }
        public bool Equals(Weather other)
        {
            if (other == null)
                return false;
            if (string.Compare(main, other.main) == 0 && string.Compare(description, other.description) == 0
                && string.Compare(icon, other.icon) == 0)
                return true;
            else
                return false;
        }
    }
    [Table("OpenWeather")]
    public class Forecast : IEquatable<Forecast>
    {
        [Key, ForeignKey("OpenWeather"), Column(Order = 1)]
        public string City { get; set; }
        [Key, ForeignKey("OpenWeather"), Column(Order = 0)]
        public int CityID { get; set; }
        public OpenWeather OpenWeather { get; set; }

        [JsonProperty("temp")]
        public double Temp { get; set; }
        [JsonProperty("pressure")]
        public int Pressure { get; set; }
        [JsonProperty("humidity")]
        public int Humidity { get; set; }
        [JsonProperty("temp_min")]
        public double TempMin { get; set; }
        [JsonProperty("temp_max")]
        public double TempMax { get; set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as Forecast);
        }
        public override int GetHashCode()
        {
            return Temp.GetHashCode() ^ Pressure.GetHashCode() 
                ^ Humidity.GetHashCode() ^ TempMin.GetHashCode() ^ TempMax.GetHashCode();
        }
        public bool Equals(Forecast other)
        {
            if (other == null)
                return false;
            if (Temp == other.Temp && Pressure == other.Pressure &&
                Humidity == other.Humidity && TempMin == other.TempMin && TempMax == other.TempMax)
                return true;
            else
                return false;
        }
    }
    [Table("OpenWeather")]
    public class Wind : IEquatable<Wind>
    {
        [Key, ForeignKey("OpenWeather"), Column(Order = 1)]
        public string City { get; set; }
        [Key, ForeignKey("OpenWeather"), Column(Order = 0)]
        public int CityID { get; set; }
        public OpenWeather OpenWeather { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }
        [JsonProperty("deg")]
        public int Direction { get; set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as Wind);
        }
        public override int GetHashCode()
        {
            return Speed.GetHashCode() ^ Direction.GetHashCode();
        }
        public bool Equals(Wind other)
        {
            if (other == null)
                return false;
            if (Speed == other.Speed && Direction == other.Direction)
                return true;
            else
                return false;
        }
    }
    [Table("OpenWeather")]
    public class Clouds : IEquatable<Clouds>
    {
        [Key, ForeignKey("OpenWeather"), Column(Order = 1)]
        public string City { get; set; }
        [Key, ForeignKey("OpenWeather"), Column(Order = 0)]
        public int CityID { get; set; }
        public OpenWeather OpenWeather { get; set; }

        [JsonProperty("all")]
        public int Cloudiness { get; set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as Clouds);
        }
        public override int GetHashCode()
        {
            return Cloudiness.GetHashCode() ^ 32;
        }
        public bool Equals(Clouds other)
        {
            if (other == null)
                return false;
            if (Cloudiness == other.Cloudiness)
                return true;
            else
                return false;
        }
    }
}
