using System;
using System.Collections.Generic;
using EasyRabbit.Consuming;
using EasyRabbit.Options;
using EasyRabbit.Producting;
using EasyRabbit.Utils;

namespace EasyRabbit
{
    public class RabbitMQBuilder
    {
        private readonly List<ConsumeBuilder> _consumeBuilders;
        private readonly List<PublishBuilder> _publishBuilders;
        private Action<Type> _registerConsumeHandlerAction;

        public RabbitMQBuilder()
        {
            _consumeBuilders = new List<ConsumeBuilder>();
            _publishBuilders = new List<PublishBuilder>();
        }

        public IReadOnlyList<ConsumeBuilder> ConsumeBuilders => _consumeBuilders;

        public IReadOnlyList<PublishBuilder> PublishBuilders => _publishBuilders;

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