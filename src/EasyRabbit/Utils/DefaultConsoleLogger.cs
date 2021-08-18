using System;

namespace EasyRabbit.Utils
{
    public class DefaultConsoleLogger : ILogger
    {
        public void Debug(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Error(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Info(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Wran(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}