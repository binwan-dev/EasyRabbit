using System;
using System.Collections.Generic;
using EasyRabbit.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRabbit
{
    public class ConsumerBuilder<TMessaging>
    {
        private Consumer<TMessaging> _consumer;
        private RabbitBuilder _builder;

        public ConsumerBuilder(RabbitBuilder builder)
        {
            _consumer = new Consumer<TMessaging>();
            _builder = builder;
        }

        public ConsumerBuilder<TMessaging> SetName(string name)
        {
            _consumer.Queue.Name = name;
            return this;
        }

        public ConsumerBuilder<TMessaging> SetVirtualHost(string virtualHost)
        {
            _consumer.Queue.VirtualHost = virtualHost;
            return this;
        }

        public ConsumerBuilder<TMessaging> SetRoutingKey(string routingKey)
        {
            _consumer.Queue.RoutingKey = routingKey;
            return this;
        }

        public ConsumerBuilder<TMessaging> EnableAutoDelete()
        {
            _consumer.Queue.AutoDelete = true;
            return this;
        }

        public ConsumerBuilder<TMessaging> EnableDurable()
        {
            _consumer.Queue.Durable = true;
            return this;
        }

        public ConsumerBuilder<TMessaging> EnableExclusive()
        {
            _consumer.Queue.Exclusive = true;
            return this;
        }

        /// <summary>
        /// Set ttl
        /// <param name="ttl">unit: ms</param>
        /// </summary>
        public ConsumerBuilder<TMessaging> SetTTL(int ttl)
        {
            _consumer.Queue.Arguments.Add("x-message-ttl", ttl);
            return this;
        }

        /// <summary>
        /// Set expire time
        /// <param name="expire">unit: ms</param>
        /// </summary>
        public ConsumerBuilder<TMessaging> SetAutoExpire(int expire)
        {
            _consumer.Queue.Arguments.Add("x-expires", expire);
            return this;
        }

        public ConsumerBuilder<TMessaging> AddBinding(
            string exchange, string routingKey = "*", IDictionary<string, object> arguments = null)
        {
            _consumer.Queue.Bindings.Add(new QueueBindingOptions()
            {
                Exchange = exchange,
                RoutingKey = routingKey,
                Arguments = arguments
            });
            return this;
        }


        public ConsumerBuilder<TMessaging> ConfigQos(QosOptions options)
        {
            _consumer.Qos = options;
            return this;
        }

        public ConsumerBuilder<TMessaging> ConfigHandler<THandler>() where THandler : IMessagingHandler<TMessaging>
        {
            _consumer.Handler = (m, c) =>
            {
                var handler = RabbitBuilder.ServiceProvider.GetService<THandler>();
                if (handler == null)
                {
                    throw new ArgumentNullException($"Please register Handler[{typeof(THandler)}]");
                }
                return handler.Handle(m, c);
            };
            return this;
        }

        public Consumer<TMessaging> Build()
        {
            _builder.Consumers.Add(_consumer);
            return _consumer;
        }

    }
}
