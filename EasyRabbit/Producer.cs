using System;
using System.Collections.Generic;
using EasyRabbit.Models;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EasyRabbit
{
    public class Producer<TMessaging> 
    {
        public Producer()
        {
            var builder = new Lazy<RabbitBuilder>(()=>RabbitBuilder.ServiceProvider.GetService<RabbitBuilder>());
            Serializer = data => builder.Value.JsonSerializer.Serialize(data);
            Exchange = new ExchangeOptions();
        }

        public ExchangeOptions Exchange { get; internal set; }

        public RabbitConnection Connection { get; private set; }

        public IModel Channel { get; private set; }

        public Func<TMessaging, byte[]> Serializer { get; private set; }

        public bool UnitOfWorkMode { get; internal set; }
        
        public Producer<TMessaging> Publish(
            TMessaging message, string routingKey = null, IDictionary<string, object> headers = null, bool? mandatory = null)
        {
            if (UnitOfWorkMode)
            {
                var unitOfWork = RabbitBuilder.ServiceProvider.GetService<IPublishUnitOfWork>();
                unitOfWork.Publish(() => DoPublish(message, routingKey, headers, mandatory));
            }
            else
            {
                DoPublish(message, routingKey, headers, mandatory);
            }
            return this;
        }

        private void DoPublish(
            TMessaging message, string routingKey = null, IDictionary<string, object> headers = null, bool? mandatory = null)
        {
            if (routingKey == null)
            {
                routingKey = Exchange.RoutingKey;
            }
            if (!mandatory.HasValue)
            {
                mandatory = Exchange.Mandatory;
            }
            if (Connection == null)
            {
                InitConnection();
            }

            var properties = Channel.CreateBasicProperties();
            properties.Headers = headers;
            Channel.BasicPublish(Exchange.Name, routingKey, mandatory.Value, properties, Serializer(message));
        }

        private void InitConnection()
        {
            var setting = RabbitBuilder.ServiceProvider.GetService<RabbitBuilder>().ServerOptions;
            var connectionPool = RabbitBuilder.ServiceProvider.GetService<RabbitConnectionPool>();
            Connection = connectionPool.GetConnection(setting, Exchange.VirtualHost);
            Connection.Connect();
            Channel = Connection.BaseConnection.CreateModel();
            JsonConvert.SerializeObject(Exchange);
            Channel.ExchangeDeclare(Exchange.Name, Exchange.Type, Exchange.Durable, Exchange.AutoDelete, Exchange.Arguments);
        }
    }
}
