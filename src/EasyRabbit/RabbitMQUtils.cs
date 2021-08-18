using EasyRabbit.Options;
using RabbitMQ.Client;

namespace EasyRabbit
{
    public class RabbitMQUtils
    {
        public static IConnection CreateNewConnection(ServerOptions serverOptions, string virtualHost)
        {
            var factory = new ConnectionFactory()
            {
                HostName = serverOptions.Host,
                Port = serverOptions.Port,
                UserName = serverOptions.UserName,
                Password = serverOptions.Password,
                RequestedHeartbeat = new System.TimeSpan(0, 0, serverOptions.RequestedHeartbeat),
                AutomaticRecoveryEnabled = true,
                VirtualHost = virtualHost
            };
            return factory.CreateConnection();
        }
    }
}
