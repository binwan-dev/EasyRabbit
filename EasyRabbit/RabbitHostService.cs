using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRabbit
{
    public class RabbitHostService : IHostedService
    {
        private readonly ILogger<RabbitHostService> _log;
        private readonly RabbitBuilder _builder;
        private readonly IServiceProvider _serviceProvider;
        private static bool _isStarted = false;
        private RabbitConnectionPool _connectionPool;

        public RabbitHostService(ILogger<RabbitHostService> log, RabbitBuilder builder, IServiceProvider serviceProvider)
        {
            _log = log;
            _builder = builder;
            _connectionPool = serviceProvider.GetService<RabbitConnectionPool>();
            RabbitBuilder.ServiceProvider=serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_isStarted)
            {
                return Task.CompletedTask;
            }
            _isStarted = true;

            foreach (var customer in _builder.Consumers)
            {
                var connection = _connectionPool.GetConnection(_builder.ServerOptions, customer.Queue.VirtualHost);
                connection.Connect();
                customer.TryBind(connection);
            }
            _log.LogInformation("EasyRabbit is started!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_isStarted)
            {
                return Task.CompletedTask;
            }
            _isStarted = false;

            _connectionPool.Dispose();
            _log.LogInformation("EasyRabbit is stopped!");
            return Task.CompletedTask;
        }
    }
}
