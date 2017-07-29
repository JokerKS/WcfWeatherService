using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;

namespace WcfWeatherService
{
    class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(8)]
        public string ClientId { get; set; }
        [StringLength(25, MinimumLength = 5)]
        public string Login { get; set; }
        [StringLength(25, MinimumLength = 5)]
        public string Password { get; set; }
    }

    class UniqueIdGenerator
    {
        public static string GenerateUniqueId(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();
            Thread.Sleep(50);
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
