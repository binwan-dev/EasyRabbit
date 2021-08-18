using System;
using System.Collections.Generic;
using EasyRabbit.Options;
using RabbitMQ.Client;

namespace EasyRabbit
{
    public class RabbitMQConnectionFactory
    {
        public static readonly RabbitMQConnectionFactory Instance = new RabbitMQConnectionFactory();
        private readonly IDictionary<string, RabbitMQConnection> _connectionDic;

        public RabbitMQConnectionFactory()
        {
            _connectionDic = new Dictionary<string, RabbitMQConnection>();
        }

        public RabbitMQConnection GetOrCreateConnection(ServerOptions options, string virtualHost, Action<RabbitMQConnection> connected)
        {
            var key = $"{options.Host}:{options.Port}/{virtualHost}";
            if (_connectionDic.TryGetValue(key, out RabbitMQConnection connection))
                return connection;

            connection = new RabbitMQConnection(options, virtualHost, connected);
            _connectionDic.Add(key, connection);
            return connection;
        }
    }
}