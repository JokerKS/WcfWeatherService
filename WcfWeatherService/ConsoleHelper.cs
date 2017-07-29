using System;

namespace WcfWeatherService
{
    static class Output
    {
        public static void ShowError(string message)
        {
            ShowText(ConsoleColor.Red, $"Error: {message}");
        }
        public static void ShowInfo(string message)
        {
            ShowText(ConsoleColor.Blue, message);
        }
        public static void ShowResult(string message)
        {
            ShowText(ConsoleColor.Green, message);
        }
        private static void ShowText(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
