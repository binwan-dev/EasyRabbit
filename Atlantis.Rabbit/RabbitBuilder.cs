using System;
using System.Collections.Generic;
using System.Reflection;
using Atlantis.Rabbit.Models;
using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Atlantis.Rabbit
{
    public class RabbitBuilder
    {
        private static IServiceProvider _serviceProvider;
        private readonly IServiceCollection _services;

        public RabbitBuilder(IServiceCollection services)
        {
            JsonSerializer = new DefaultSerializer();
            _services = services;
            Consumers = new List<IConsumer>();
        }

        public RabbitServerSetting ServerOptions { get; set; }

        public ISerializer JsonSerializer { get; set; }

        public ISerializer BinarySerializer { get; set; }

        internal IList<IConsumer> Consumers { get; set; }

        internal IServiceCollection Services => _services;

        public ConsumerBuilder<TMessaging> CreateConsumer<TMessaging>()
        {
            return new ConsumerBuilder<TMessaging>(this);
        }

        public ProducerBuilder<TMessaging> CreateProducer<TMessaging>()
        {
            return new ProducerBuilder<TMessaging>(this);
        }

        internal static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    throw new InvalidOperationException("Please call UseRabbit Method with IServiceProvider");
                }
                return _serviceProvider;
            }
            set
            {
                _serviceProvider = value;
            }
        }

    }
}
