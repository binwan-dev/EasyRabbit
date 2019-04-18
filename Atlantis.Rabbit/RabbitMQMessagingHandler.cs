using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Atlantis.Rabbit
{
    public abstract class RabbitMQMessagingHandler<TMessage> :IRabbitMessagingHandler
    {
        private readonly ILogger<RabbitMQMessagingHandler<TMessage>> _log;
        private readonly ISerializer _serialzer;

        public RabbitMQMessagingHandler()
        {
            var serviceProvider=RabbitConfigurationExetension.ServiceProvider;
            _log=serviceProvider.GetService<ILogger<RabbitMQMessagingHandler<TMessage>>>();
            _serialzer=serviceProvider.GetService<ISerializer>();
        }

        public abstract string Queue { get; }

        public abstract string Exchange { get; }

        public virtual string RoutingKey { get; set; }

        public virtual string VirtualHost { get; set; }

        public virtual bool IsEnable => true;

        public virtual string Name { get; set; }

        public virtual long TTL{get;set;}

        public async Task Handle(RabbitConnection connection, object model, BasicDeliverEventArgs e)
        {
            TMessage message;
            try
            {
                message = Deserialize(e.Body);
                if (message == null)
                {
                    _log.LogWarning($"队列：{Queue}，无法序列化消息，已跳过该条消息！消息数据：{GetErrorByteBody(e.Body)}");
                    connection.AckMessage(e.DeliveryTag);
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"队列：{Queue}，无法序列化消息，已跳过该条消息！");
                connection.AckMessage(e.DeliveryTag);
                return;
            }

            try
            {
                _log.LogInformation($"队列：{Queue}，接收到MQ消息：{_serialzer.Serialize(message)}");
                await Handle(message);
                connection.AckMessage(e.DeliveryTag);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"系统在执行{typeof(TMessage)}消息的时候出现异常！原因：{ex.Message}");
                Thread.Sleep(new Random().Next(1000, 30000));
                connection.RejectMessage(e.DeliveryTag);
            }
        }

        protected abstract Task Handle(TMessage message);

        protected virtual TMessage Deserialize(byte[] messageBody)
        {
            TMessage t = _serialzer.Deserialize<TMessage>(messageBody);
            return t;
        }

        protected virtual TMessage DeserializeByJson(byte[] messageBody)
        {
            return _serialzer.Deserialize<TMessage>(messageBody);
        }
        
        private string GetErrorByteBody(byte[] body)
        {
            if (body == null || body.Length == 0) return string.Empty;
            var str = new StringBuilder();
            foreach (var item in body)
            {
                str.Append($"{str},");
            }
            return str.ToString();
        }
    }
}
