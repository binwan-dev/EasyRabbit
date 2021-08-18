using System;

namespace EasyRabbit.Utils
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger<T>();

        ILogger CreateLogger(Type type);
    }
}