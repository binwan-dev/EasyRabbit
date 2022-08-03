using System;
using Microsoft.Extensions.Logging;
using IEasyLogger = EasyRabbit.Utils.ILogger;

namespace EasyRabbit.AspNetCore
{
    public class MicrosoftLogger : IEasyLogger
    {
        private readonly ILogger _logger;

        public MicrosoftLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void Wran(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
