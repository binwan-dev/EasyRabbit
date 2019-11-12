using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading;
using Atlantis.Rabbit.Models;

namespace Atlantis.Rabbit
{
    public class Consumer<TMessaging> : IConsumer
    {
        private readonly Lazy<RabbitBuilder> _builder;
        private readonly Lazy<ILogger<Consumer<TMessaging>>> _log;
        private int _binded = 0;
        private int _rebindTimes = 0;

        public Consumer()
        {
            _builder = new Lazy<RabbitBuilder>(() => RabbitBuilder.ServiceProvider.GetService<RabbitBuilder>());
            _log = new Lazy<ILogger<Consumer<TMessaging>>>(() => RabbitBuilder.ServiceProvider.GetService<ILogger<Consumer<TMessaging>>>());
            Deserializer = data => _builder.Value.JsonSerializer.Deserialize<TMessaging>(data);
            Queue = new QueueOptions();
            Qos = QosOptions.Default();
        }

        public QueueOptions Queue { get; set; }

        public QosOptions Qos { get; set; }

        public Func<TMessaging, ConsumerMessagingContext<TMessaging>, Task> Handler { get; set; }

        public RabbitConnection Connection { get; private set; }

        public IModel Channel { get; private set; }

        public Func<byte[], TMessaging> Deserializer { get; private set; }

        public void RejectMessage(ulong deliveryTag)
        {
            if (!Connection.BaseConnection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message reject failed! message deliverytag: {deliveryTag}");
            Channel.BasicReject(deliveryTag, true);
        }

        public void AckMessage(ulong deliveryTag)
        {
            if (!Connection.BaseConnection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message ack failed! message deliverytag: {deliveryTag}");
            Channel.BasicAck(deliveryTag, false);
        }

        void IConsumer.TryBind(RabbitConnection connection)
        {
            TryBind(connection);
        }


        private void TryBind(RabbitConnection connection)
        {
            if (!EnterBinding()) return;

            try
            {
                ReBind(connection);
            }
            catch (Exception ex)
            {
                _log.Value.LogError(ex, $"The queue bind failed! the reason is: {ex.Message}");
                ExitReBind();

                var rebindTimeMillisecond = _builder.Value.ServerOptions.ReBindTimeMillisecond;
                var sleepingTime = rebindTimeMillisecond * _rebindTimes;
                _log.Value.LogInformation($"The queue rebind after {sleepingTime / 1000} s!");
                Thread.Sleep(sleepingTime);
                if (_rebindTimes <= 10)
                {
                    _rebindTimes++;
                }
                TryBind(connection);
                return;
            }
            ExitReBind();
        }

        private void Binding(RabbitConnection connection)
        {
            try
            {
                Channel = connection.BaseConnection.CreateModel();
                Connection = connection;

                Channel.QueueDeclare(Queue.Name, Queue.Durable, Queue.Exclusive, Queue.AutoDelete, Queue.Arguments);

                foreach (var binding in Queue.Bindings)
                {
                    Channel.QueueBind(Queue.Name, binding.Exchange, binding.RoutingKey, binding.Arguments);
                    _log.Value.LogInformation($"The Channel binding success! queue(name: {Queue.Name}, exchange: {binding.Exchange}), routingkey: {binding.RoutingKey} at rabbit mq! ");
                }
                Channel.BasicQos(Qos.PrefetchSize, Qos.PrefetchCount, Qos.Global);
                var consume = new EventingBasicConsumer(Channel);
                consume.Received += async (model, e) =>
                {
                    using (var scope = RabbitBuilder.ServiceProvider.CreateScope())
                    {
                        var message = _builder.Value.JsonSerializer.Deserialize<TMessaging>(e.Body);
                        var context = new ConsumerMessagingContext<TMessaging>()
                        {
                            Consumer = this,
                            Properties = e
                        };
                        await Handler(message, context);
                    }
                };
                Channel.BasicConsume(Queue.Name, false, consume);
                _log.Value.LogInformation($"Queue bind success! Queue[{JsonConvert.SerializeObject(Queue)}]");
            }
            catch (Exception ex)
            {
                _log.Value.LogError(ex, $"Can not binding queue({Queue.Name} virtualhost({Queue.VirtualHost}) at rabbit mq! reason: {ex.Message} Data: {JsonConvert.SerializeObject(Queue)}");
                throw new InvalidOperationException("Binding failed!");
            }
        }

        private void ReBind(RabbitConnection connection)
        {
            Binding(connection);
            _rebindTimes = 1;
        }

        private bool EnterBinding()
        {
            return Interlocked.CompareExchange(ref _binded, 1, 0) == 0;
        }

        private void ExitReBind()
        {
            Interlocked.Exchange(ref _binded, 0);
        }
    }
}
