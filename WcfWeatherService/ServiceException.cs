using System.Runtime.Serialization;

namespace WcfWeatherService
{
    [DataContract]
    public class ServiceException
    {
        [DataMember]
        public string Message { get; set; }
        public ServiceException()
        {

        }
        public ServiceException(string message)
        {
            Message = message;
        }
    }

    [DataContract]
    public class ServerHasNoWeatherData : ServiceException
    {
        public ServerHasNoWeatherData()
        {

        }
        public ServerHasNoWeatherData(string message) : base(message)
        {

        }
    }
    [DataContract]
    public class CityNotFound : ServiceException
    {
        public CityNotFound()
        {

        }
        public CityNotFound(string message) : base(message)
        {

        }
    }
    [DataContract]
    public class CountryNotFound : ServiceException
    {
        public CountryNotFound()
        {

        }
        public CountryNotFound(string message) : base(message)
        {

        }
    }
    [DataContract]
    public class UniqueClientIdNotFound : ServiceException
    {
        public UniqueClientIdNotFound()
        {

        }
        public UniqueClientIdNotFound(string message) : base(message)
        {

        }
    }
    [DataContract]
    public class NotCorrectLoginOrPassword : ServiceException
    {
        public NotCorrectLoginOrPassword()
        {

        }
        public NotCorrectLoginOrPassword(string message) : base(message)
        {

        }
    }
}
