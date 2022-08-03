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

        public RabbitMQConnection GetOrCreateConnection(ServerOptions options, string virtualHost, Action<RabbitMQConnection> connected = null)
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
			    connected(connection);
                        }
                    }
		}
                return connection;
            }

            lock (_newLock)
            {
		if(_connectionDic.ContainsKey(key))
                    return GetOrCreateConnection(options, virtualHost, connected);

                connection = new RabbitMQConnection(options, virtualHost);
                connection.Connected += connected;
                connection.Connect();
                _connectionDic.TryAdd(key, connection);
            }

            return connection;
        }
    }
}
