using System.Collections.Generic;
using EasyRabbit.Consuming;
using EasyRabbit.Options;
using EasyRabbit.Producting;
using EasyRabbit.Utils;

namespace EasyRabbit
{
    public class RabbitMQBuilder
    {
        private readonly IList<ConsumeBuilder> _consumeBuilders;
        private readonly IList<PublishBuilder> _publishBuilders;

        public RabbitMQBuilder()
        {
            _consumeBuilders = new List<ConsumeBuilder>();
            _publishBuilders = new List<PublishBuilder>();
        }

        public RabbitMQBuilder RegisterObjectContainer(IObjectContainer objectContainer)
        {
            ObjectContainerFactory.RegisterContainer(objectContainer);
            return this;
        }

        public RabbitMQBuilder AddGlobalServerOptions(ServerOptions serverOptions)
        {
            ServerOptions.Default = serverOptions;
            return this;
        }

        public ConsumeBuilder AddConsumer()
        {
            var builder = new ConsumeBuilder();
            _consumeBuilders.Add(builder);
            return builder;
        }

        public PublishBuilder AddProducer()
        {
            var builder = new PublishBuilder();
            _publishBuilders.Add(builder);
            return builder;
        }

        public void Start()
        {
            foreach (var builder in _consumeBuilders)
            {
                var metadata = new ConsumeMetadata()
                {
                    ServerOptions = builder.ServerOptions,
                    ConsumeOptions = builder.ConsumeOptions,
                    HandlerType = builder.HandlerType
                };
                new ConsumeChannel(metadata);
                ConsumeMetadataFactory.Set(metadata);
            }

            foreach (var builder in _publishBuilders)
            {
                var metadata = new PublishMessagingMetadata(builder.MessageType, builder.ServerOptions, builder.PublishOptions);
                PublishMessagingMetadataFactory.AddMessagingMetadata(metadata);
            }
        }

    }
}