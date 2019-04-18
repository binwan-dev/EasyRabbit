using System.Collections.Generic;
using System.Reflection;

namespace Atlantis.Rabbit
{
    public class RabbitMessagePublisher : IRabbitMessagePublisher
    {
        private readonly IDictionary<string, RabbitPublishMessageMetadata> _publishDic;

        public RabbitMessagePublisher(RabbitBuilder builder)
        {
            _publishDic = new Dictionary<string, RabbitPublishMessageMetadata>();
            // _rabbitMQSetting = builder.ServerOptions;
        }

        public void Publish<T>(T message)
        {
            // Ensure.NotNull(message, "The message cannot be null, send to rabbitmq failed!");
            // var metadata = GetOrCreateNewPublishInfo(message);
            // Ensure.NotNull(metadata, "The message found metadata error!");
            // var wrapperMessage = WrappingMessage(message, metadata);
            // Ensure.NotNull(wrapperMessage, "Wrapper message failed!");
            // SendMessage(wrapperMessage);
        }

        public void SendMessage<T>(IRabbitPublishMessageWrapper<T> message)
        {
            using (var connection = RabbitMQUtils.CreateNewConnection(message.Metadata.ConnectSetting))
            using (var channel = connection.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                channel.BasicPublish(message.Metadata.Exchange, message.Metadata.RoutingKey, false, properties, message.Serialize());
            }
        }

        private IRabbitPublishMessageWrapper<T> WrappingMessage<T>(T message, RabbitPublishMessageMetadata metadata)
        {
            switch (metadata.SerializeType)
            {
                case PublishMessageSerialzeType.Proto: return new RabbitPublishMessageProtoWrapper<T>(message,metadata);
                case PublishMessageSerialzeType.Json: return new RabbitPublishMessageJsonWrapper<T>(message, metadata);
            }
            return new RabbitPublishMessageProtoWrapper<T>(message, metadata);
        }

        private RabbitPublishMessageMetadata GetOrCreateNewPublishInfo<T>(T message)
        {
            // if (_publishDic.TryGetValue(message.GetType().FullName.ToLower(), out var metadata)&&metadata!=null) return metadata;

            // var attribute = message.GetType().GetCustomAttribute<RabbitPublishToAttribute>();
            // Ensure.NotNull(attribute, "The message cannot be found metadata info, please set RabbitPublishToAttribute to it!");

            // var hostSetting = _rabbitMQSetting.GetServer(attribute.GroupName);
            // Ensure.NotNull(hostSetting, "The message cannot be found host setting, please set it at the RabbitPublishToAttribute!");

            // metadata = new RabbitPublishMessageMetadata(attribute, hostSetting);
            // _publishDic.Add(message.GetType().FullName.ToLower(), metadata);
            // return metadata;
            throw new System.NotImplementedException();
        }
    }
}
