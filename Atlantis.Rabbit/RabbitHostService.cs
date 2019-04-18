using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Atlantis.Rabbit
{
    public class RabbitHostService : IHostedService
    {
        private readonly ILogger<RabbitHostService> _log;
        private readonly RabbitBuilder _builder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<RabbitConnection> _connections;

        public RabbitHostService(
            ILogger<RabbitHostService> log,
            RabbitBuilder builder)
        {
            _log=log;
            _builder=builder;
            _serviceProvider=RabbitConfigurationExetension.ServiceProvider;
            _connections=new List<RabbitConnection>();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var type in _builder.Metadatas)
            {
                var instance = (IRabbitMessagingHandler)_serviceProvider.GetService(type);
                if (!instance.IsEnable)
                {
                    continue;
                }
                //if (string.IsNullOrWhiteSpace(instance.Name))
                //{
                //    throw new ArgumentNullException($"The virtualhost is null in the handler({instance.GetType().Name})!");
                //}
                var hostSetting = _builder.ServerOptions;
                var rabbitContext = new RabbitConnection(instance, hostSetting, _serviceProvider);
                rabbitContext.Start();
                _connections.Add(rabbitContext);
            }
            _builder.Metadatas.Clear();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var connection in _connections)
            {
                connection.Close();
            }
            _connections.Clear();
            return Task.CompletedTask;
        }
    }
}
