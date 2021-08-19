using System;
using EasyRabbit.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EasyRabbit.Consuming
{
    public class ConsumeChannel
    {
        private readonly ConsumeMetadata _metadata;
        private RabbitMQConnection _connection;
        private readonly ILogger _logger;
        private IModel _receiveChannel;

        public ConsumeChannel(ConsumeMetadata metadata)
        {
            if (metadata.ConsumeOptions == null)
                throw new ArgumentNullException(nameof(metadata.ConsumeOptions));
            if (string.IsNullOrWhiteSpace(metadata.ConsumeOptions.Queue))
                throw new ArgumentNullException(nameof(metadata.ConsumeOptions.Queue));
            if (string.IsNullOrWhiteSpace(metadata.ConsumeOptions.Exchange))
                throw new ArgumentNullException(nameof(metadata.ConsumeOptions.Exchange));
            if (metadata.HandlerType == null)
                throw new ArgumentNullException(nameof(metadata.HandlerType));
            if (metadata.ServerOptions == null)
                throw new ArgumentNullException(nameof(metadata.ServerOptions));
            if (string.IsNullOrWhiteSpace(metadata.ServerOptions.Host))
                throw new ArgumentNullException(nameof(metadata.ServerOptions.Host));
            if (metadata.ServerOptions.Port == 0)
                throw new ArgumentNullException(nameof(metadata.ServerOptions.Port));

            _metadata = metadata;
            _logger = ObjectContainerFactory.ObjectContainer.Resolve<ILoggerFactory>().CreateLogger<ConsumeChannel>();
            _connection = RabbitMQConnectionFactory.Instance.GetOrCreateConnection(_metadata.ServerOptions, _metadata.ConsumeOptions.VirtualHost, Binding);
        }

        public RabbitMQConnection Connection => _connection;

        public IModel ReceiveChannel => _receiveChannel;

        public void Binding(RabbitMQConnection connection)
        {
            try
            {
                _receiveChannel = connection.Connection.CreateModel();
                _receiveChannel.QueueDeclare(_metadata.ConsumeOptions.Queue, true, false, false, null);
                _receiveChannel.QueueBind(_metadata.ConsumeOptions.Queue, _metadata.ConsumeOptions.Exchange, _metadata.ConsumeOptions.RoutingKey, null);
                _receiveChannel.BasicQos(0, _metadata.ConsumeOptions.PrefetchCount, false);
                var consume = new EventingBasicConsumer(_receiveChannel);
                consume.Received += async (model, e) =>
                {
                    var handler = ObjectContainerFactory.ObjectContainer.Resolve<IConsumeMessagingHandler>(_metadata.HandlerType);
                    await handler.HandleAsync(this, model, e);
                };
                _receiveChannel.BasicConsume(_metadata.ConsumeOptions.Queue, false, consume);
                _logger.Info("The channel binding success! queue(name: {0}, exchange: {1}), routingkey: {2} at rabbit mq! ", _metadata.ConsumeOptions.Queue, _metadata.ConsumeOptions.Exchange, _metadata.ConsumeOptions.RoutingKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can not binding queue(name: {0}, exchange: {1}), routingkey: {2} at rabbit mq!", _metadata.ConsumeOptions.Queue, _metadata.ConsumeOptions.Exchange, _metadata.ConsumeOptions.RoutingKey);
                throw new InvalidOperationException($"Can not binding queue(name: {_metadata.ConsumeOptions.Queue}, exchange: {_metadata.ConsumeOptions.Exchange}), routingkey: {_metadata.ConsumeOptions.RoutingKey} at rabbit mq!", ex);
            }
        }

    }
}