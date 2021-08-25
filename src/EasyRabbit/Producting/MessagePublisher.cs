using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using EasyRabbit.Options;
using EasyRabbit.Producting;
using EasyRabbit.Utils;
using RabbitMQ.Client;

namespace EasyRabbit.Publishes
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger _logger;
        private readonly ISerializer _jsonSerializer;

        public MessagePublisher()
        {
            _logger = ObjectContainerFactory.ObjectContainer.Resolve<ILoggerFactory>().CreateLogger<MessagePublisher>();
            _jsonSerializer = SerializeFactory.Serializer;
        }

        public void Publish<T>(T message, IDictionary<string, object> headers = null, PublishOptions publishOptions = null, ServerOptions serverOptions = null, Func<T, ReadOnlyMemory<byte>> serialize = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var metadata = PublishMessagingMetadataFactory.GetMetadata(typeof(T));
            if ((metadata == null && publishOptions == null) ||
                (metadata == null && serverOptions == null) ||
                (metadata.PublishOptions == null && publishOptions == null) ||
                (metadata.ServerOptions == null && serverOptions == null))
                throw new InvalidOperationException($"The message({typeof(T).FullName}) cannot publish! Please register this message!");
            if (_jsonSerializer == null && serialize == null)
                throw new InvalidOperationException($"The message({typeof(T).FullName}) has not default serialize, Please set or register default!");

            if (publishOptions == null)
                publishOptions = metadata.PublishOptions;
            if (serverOptions == null)
                serverOptions = metadata.ServerOptions;
            if (serialize == null)
                serialize = _jsonSerializer.Serialize;
            if (string.IsNullOrWhiteSpace(publishOptions.VirtualHost) && string.IsNullOrWhiteSpace(serverOptions.VirtualHost))
                throw new InvalidOperationException($"The message({typeof(T).FullName}) has not virtualhost!");
            if (string.IsNullOrWhiteSpace(publishOptions.VirtualHost))
                publishOptions.VirtualHost = serverOptions.VirtualHost;

            var connection = RabbitMQConnectionFactory.Instance.GetOrCreateConnection(serverOptions, publishOptions.VirtualHost);
            if (!connection.Connection.IsOpen)
                throw new InvalidOperationException($"The connection is not opened! publish failed!");

            using (var channel = connection.Connection.CreateModel())
            {
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = headers;
                channel.BasicPublish(publishOptions.Exchange, publishOptions.RoutingKey, true, basicProperties, serialize(message));
            }
        }
    }
}
