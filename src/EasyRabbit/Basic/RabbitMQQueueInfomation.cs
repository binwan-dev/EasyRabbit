using System;
using EasyRabbit.Options;

namespace EasyRabbit.Basic
{
    public class RabbitMQQueueInfomation : IRabbitMQQueueInfomation
    {
        public RabbitMQQueueInfomation()
        {
        }

        public uint GetMessageCount(string queue, ServerOptions options = null)
        {
	    if(options==null)
                options = ServerOptions.Default ?? throw new ArgumentNullException("ServerOptions");

            var connection = RabbitMQConnectionFactory.Instance.GetConnection(options, options.VirtualHost);
            using (var channel = connection.Connection.CreateModel())
            {
                return channel.QueueDeclarePassive(queue).MessageCount;
            }
        }
    }
}
