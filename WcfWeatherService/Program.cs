using System;
using System.ServiceModel;

namespace WcfWeatherService
{
    class Program
    {
        /// <summary>
        /// The launch of the service
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //To start the service, you need to:
            //1. Run VS as administrator
            //2. Register this command in CMD(as administrator) to search for the URL address
            //netsh http add urlacl url=http://localhost:8097/WcfWeatherService user=DESKTOP-JKS2017\kozen
            //where url is wcf service address, user - user name
            ServiceHost host = new ServiceHost(typeof(WeatherService));
            host.Open();
            Output.ShowResult("Service running...\nPress <Enter> to close service");
            Console.ReadLine();
            host.Close();
        }
    }
}
