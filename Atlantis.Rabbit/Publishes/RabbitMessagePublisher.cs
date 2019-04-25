using System.Collections.Generic;
using System.Reflection;
using System;

namespace Atlantis.Rabbit
{
    public class RabbitMessagePublisher : IRabbitMessagePublisher
    {
        private readonly IDictionary<string, RabbitPublishMessageMetadata> _publishDic;
        private readonly RabbitBuilder _rabbitBuilder;

        public RabbitMessagePublisher(RabbitBuilder builder)
        {
            _publishDic = new Dictionary<string, RabbitPublishMessageMetadata>();
            _rabbitBuilder=builder;
        }

        public void Publish<T>(T message,IDictionary<string,object> headers=null)
        {
            if (message == null)
            {
                throw new ArgumentNullException("The message cannot be null, send to rabbitmq failed!");
            }
            var metadata = GetOrCreateNewPublishInfo(message);
            if (metadata == null)
            {
                throw new ArgumentNullException("The message found metadata error!");
            }
            SendMessage(message,metadata,headers);
        }

        public void SendMessage<T>(
            T message,RabbitPublishMessageMetadata metadata,IDictionary<string,object> headers=null)
        {
            using (var connection = RabbitMQUtils.CreateNewConnection(metadata.ConnectSetting))
            using (var channel = connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.Headers=headers;
                channel.BasicPublish(
                    metadata.Exchange, metadata.RoutingKey, 
                    false, properties, Serialize(message,metadata));
            }
        }

        protected byte[] Serialize<T>(T message,RabbitPublishMessageMetadata metadata)
        {
            if(metadata.SerializeType==PublishMessageSerialzeType.Json)
            {
                return _rabbitBuilder.JsonSerializer.Serialize(message);
            }
            if(metadata.SerializeType==PublishMessageSerialzeType.Proto)
            {
                return _rabbitBuilder.BinarySerializer.Serialize(message);
            }
            throw new NotImplementedException();
        }

        private RabbitPublishMessageMetadata GetOrCreateNewPublishInfo<T>(T message)
        {
            var msgName = message.GetType().FullName.ToLower();
            var result = _publishDic.TryGetValue(msgName, out var metadata);
            if (result && metadata != null)
            {
                return metadata;
            }

            var attribute = message.GetType().GetCustomAttribute<RabbitPublishToAttribute>();
            if (attribute == null)
            {
                throw new ArgumentNullException("The message cannot be found metadata info, please set RabbitPublishToAttribute to it!");
            }
            
            metadata = new RabbitPublishMessageMetadata(attribute, _rabbitBuilder.ServerOptions);
            _publishDic.Add(msgName, metadata);
            return metadata;
        }
    }
}
