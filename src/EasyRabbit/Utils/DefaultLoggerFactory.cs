using System;

namespace EasyRabbit.Utils
{
    public class DefaultLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger<T>()
        {
            return new DefaultConsoleLogger();
        }

        public ILogger CreateLogger(Type type)
        {
            return new DefaultConsoleLogger();
        }
    }
}