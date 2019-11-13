using Microsoft.Extensions.DependencyInjection;

namespace EasyRabbit
{
    public class ProducerBuilder<TMessaging>
    {
        private Producer<TMessaging> _producer;
        private RabbitBuilder _builder;

        public ProducerBuilder(RabbitBuilder builder)
        {
            _producer = new Producer<TMessaging>();
            _builder= builder;
        }

        public ProducerBuilder<TMessaging> SetName(string name)
        {
            _producer.Exchange.Name = name;
            return this;
        }

        public ProducerBuilder<TMessaging> SetType(Models.ExchangeType type)
        {
            _producer.Exchange.Type = type.ToString().ToLower();
            return this;
        }

        public ProducerBuilder<TMessaging> SetRoutingKey(string routingKey)
        {
            _producer.Exchange.RoutingKey=routingKey;
            return this;
        }

        public ProducerBuilder<TMessaging> EnableAutoDelete()
        {
            _producer.Exchange.AutoDelete=true;
            return this;
        }

        public ProducerBuilder<TMessaging> EnableDurable()
        {
            _producer.Exchange.Durable=true;
            return this;
        }

        public ProducerBuilder<TMessaging> EnableInternal()
        {
            _producer.Exchange.Internal=true;
            return this;
        }
        
        public ProducerBuilder<TMessaging> SetAlternate(string exchange)
        {
            _producer.Exchange.Arguments.Add("alternate-exchange", exchange);
            return this;
        }


        public ProducerBuilder<TMessaging> EnableUnitOfWorkMode()
        {
            _producer.UnitOfWorkMode = true;
            return this;
        }

        public Producer<TMessaging> Build()
        {
            _builder.Services.AddSingleton<Producer<TMessaging>>(_producer);
            return _producer;
        }

    }
}
