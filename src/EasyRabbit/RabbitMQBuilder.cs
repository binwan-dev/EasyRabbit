using System.Collections.Generic;
using EasyRabbit.Consuming;
using EasyRabbit.Options;
using EasyRabbit.Utils;

namespace EasyRabbit
{
    public class RabbitMQBuilder
    {
        private readonly IList<ConsumeBuilder> _consumeBuilders;

        public RabbitMQBuilder()
        {
            _consumeBuilders = new List<ConsumeBuilder>();
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

        public ConsumeBuilder AddConsume()
        {
            var builder = new ConsumeBuilder();
            _consumeBuilders.Add(builder);
            return builder;
        }
    }
}