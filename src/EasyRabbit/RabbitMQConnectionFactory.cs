using System;
using System.Collections.Concurrent;
using EasyRabbit.Options;

namespace EasyRabbit
{
    public class RabbitMQConnectionFactory
    {
        public static readonly RabbitMQConnectionFactory Instance = new RabbitMQConnectionFactory();
        private readonly ConcurrentDictionary<string, RabbitMQConnection> _connectionDic;
        private static object _connectLock = new object();
	private static object _newLock = new object();

        public RabbitMQConnectionFactory()
        {
            _connectionDic = new ConcurrentDictionary<string, RabbitMQConnection>();
        }

        public RabbitMQConnection GetConnection(ServerOptions options, string virtualHost)
        {
            var key = $"{options.Host}:{options.Port}/{virtualHost}";
            if (_connectionDic.TryGetValue(key, out RabbitMQConnection connection))
            {
                if (!connection.Connection.IsOpen)
		{
		    lock (_connectLock)
		    {
                        if (!connection.Connection.IsOpen)
                        {
                            connection.Connect();
                        }
                    }
		}
                return connection;
            }

            lock (_newLock)
            {
		if(_connectionDic.ContainsKey(key))
                    return GetConnection(options, virtualHost);

                connection = new RabbitMQConnection(options, virtualHost);
                connection.Connect();
                _connectionDic.TryAdd(key, connection);
            }

            return connection;
        }
    }
}
