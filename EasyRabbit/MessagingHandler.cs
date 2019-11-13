using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyRabbit
{
    public abstract class MessagingHandler<TMessaging> : IMessagingHandler<TMessaging>
    {
        private readonly ILogger<MessagingHandler<TMessaging>> _log;
        private readonly RabbitBuilder _builder;
        private readonly Producer<TMessaging> _producer;

        public MessagingHandler()
        {
            var serviceProvider = RabbitBuilder.ServiceProvider;
            _log = serviceProvider.GetService<ILogger<MessagingHandler<TMessaging>>>();
            _builder = serviceProvider.GetService<RabbitBuilder>();
            _producer = serviceProvider.GetService<Producer<TMessaging>>();
        }

        public async Task Handle(TMessaging message, ConsumerMessagingContext<TMessaging> context)
        {
            try
            {
                _log.LogInformation($"Queue[{context.Consumer.Queue.Name}]，Receive msg：{_builder.JsonSerializer.SerializeStr(message)}");
                await Process(message, context);
                context.Consumer.AckMessage(context.Properties.DeliveryTag);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Execute msg have error! Queue[{context.Consumer.Queue.Name}] Message[{JsonConvert.SerializeObject(message)}] reason：{ex.Message}");
                Thread.Sleep(new Random().Next(1000, 30000));
                Requeue(message, context);
            }
        }

        protected abstract Task Process(TMessaging message, ConsumerMessagingContext<TMessaging> context);

        protected void Requeue(TMessaging message, ConsumerMessagingContext<TMessaging> context)
        {
            var headers = new Dictionary<string, object>();
            headers.Add("Type", "Failed-Retry");

            _producer.Publish(message,headers:headers);
            context.Consumer.AckMessage(context.Properties.DeliveryTag);
        }
    }
}
