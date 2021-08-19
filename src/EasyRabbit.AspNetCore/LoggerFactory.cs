using System;
using IEasyLogger = EasyRabbit.Utils.ILogger;
using IEasyLoggerFactory = EasyRabbit.Utils.ILoggerFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyRabbit.AspNetCore
{
    public class MicrosoftLoggerFactory : IEasyLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public MicrosoftLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IEasyLogger CreateLogger<T>()
        {
            var logger = _loggerFactory.CreateLogger(typeof(T).Name);
            return new MicrosoftLogger(logger);
        }

        public IEasyLogger CreateLogger(Type type)
        {
            var logger = _loggerFactory.CreateLogger(type.Name);
            return new MicrosoftLogger(logger);
        }
    }
}