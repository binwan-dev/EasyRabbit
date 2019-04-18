using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlantis.Rabbit
{
    public class RabbitConnection
    {
        private readonly RabbitServerSetting _setting;
        private readonly Func<RabbitConnection, object, BasicDeliverEventArgs,Task> _receiveAction;
        private IConnection _connection;
        private readonly ILogger<RabbitConnection> _log;
        private readonly ISerializer _serializer;
        private IModel _receiveChannel;
        private readonly string _receiveQueue;
        private readonly string _receiveExchange;
        private readonly string _routingKey;
        private readonly long _ttl;

        private int _connecting = 0;
        private int _reconnectTimes=1;

        private Type _handlerType;

        public RabbitConnection(
            IRabbitMessagingHandler handler,RabbitServerSetting setting,IServiceProvider serviceProvider)
        {
            _receiveQueue = handler.Queue ?? throw new ArgumentNullException("The queue must be declare!");
            _receiveExchange = handler.Exchange ?? throw new ArgumentNullException("The exchange must be declare!");
            _setting = setting ?? throw new ArgumentNullException("The rabbitmq host setting must be declare!");
            _receiveAction = handler.Handle;
            _routingKey=handler.RoutingKey??"#";
            _handlerType=handler.GetType();
            _ttl=handler.TTL;

            if(!string.IsNullOrWhiteSpace(handler.VirtualHost))
            {
                _setting.VirtualHost=handler.VirtualHost;
            }

            _log = serviceProvider.GetService<ILogger<RabbitConnection>>();
            _serializer = serviceProvider.GetService<ISerializer>();
        }

        public IConnection Connection => _connection;

        public string ReceiveQueue => _receiveQueue;

        public string ReceiveExchange => _receiveExchange;

        public string RoutingKey=>_routingKey;

        public IModel ReceiveChannel => _receiveChannel;

        public void Start(bool isAutoConnect=true)
        {
            try
            {
                _connection = RabbitMQUtils.CreateNewConnection(_setting);
                Binding();
            }
            catch (Exception ex)
            {
                _log.LogError(ex,$"The rabbit mq service cannot connect! reason: {ex.Message}");
                if (isAutoConnect)
                {
                    _log.LogInformation($"Auto connect is enable, the service will reconnect to the rabbit mq server after {_setting.ReconnectTimeMillisecond} ms!");
                    TryReConnect();
                }
            }
        }

        public void RejectMessage(ulong deliveryTag)
        {
            if (!_connection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message reject failed! message deliverytag: {deliveryTag}");
            _receiveChannel.BasicReject(deliveryTag, true);
        }

        public void AckMessage(ulong deliveryTag)
        {
            if (!_connection.IsOpen) throw new InvalidOperationException($"The server isn't opened, the message ack failed! message deliverytag: {deliveryTag}");
            _receiveChannel.BasicAck(deliveryTag, false);
        }

        public void Close()
        {
            _connection.Close();
        }

        #region Reconnect
        private void TryReConnect()
        {
            if (!EnterReConnect()) return;
            Thread.Sleep(_setting.ReconnectTimeMillisecond);
            try
            {
                _log.LogInformation($"Try reconnect to server! server info: ip={_setting.Host}, port={_setting.Port}, virtual host={_setting.VirtualHost}, username={_setting.UserName}, password={_setting.Password}");
                ReConnect();
            }
            catch(Exception ex)
            {
                _log.LogError(ex,$"The connection connect failed! the reason is: {ex.Message}");
                ExitReConnect();
                var sleepingTime=_setting.ReconnectTimeMillisecond*_reconnectTimes*_reconnectTimes;
                _log.LogInformation($"The connection reconnect to server after {sleepingTime/1000} s!");
                Thread.Sleep(sleepingTime);
                if(_reconnectTimes<=30)_reconnectTimes++;
                TryReConnect();
                return;
            }
            ExitReConnect();

        }

       private void ReConnect()
        {
            _connection = RabbitMQUtils.CreateNewConnection(_setting);
            Binding();
            _reconnectTimes=1;
        }

        private bool EnterReConnect()
        {
            return Interlocked.CompareExchange(ref _connecting, 1, 0) == 0;
        }

        private void ExitReConnect()
        {
            Interlocked.Exchange(ref _connecting, 0);
        }
        #endregion

        private void Binding()
        {
            try
            {
                _connection.ConnectionShutdown+=(model,e)=>
                {
                    _log.LogWarning($"Rabbit connection has stoped! queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq!");
                };
                _receiveChannel = _connection.CreateModel();
                var dic=new Dictionary<string,object>();
                if(_ttl>0)
                {
                    dic.Add("x-message-ttl",_ttl);
                }
                _receiveChannel.QueueDeclare(_receiveQueue, true, false, false, dic);
                _receiveChannel.QueueBind(_receiveQueue, _receiveExchange, _routingKey, dic);
                _receiveChannel.BasicQos(0, 1, false);
                var consume = new EventingBasicConsumer(_receiveChannel);
                consume.Received +=(model, e) => 
                {
                    using(var scope=RabbitConfigurationExetension.ServiceProvider.CreateScope())
                    {
                        var instance=(IRabbitMessagingHandler)scope.ServiceProvider.GetService(_handlerType);
                        instance.Handle(this,model,e);
                    }
                };
                _receiveChannel.BasicConsume(_receiveQueue, false, consume);
                _log.LogInformation($"The channel binding success! queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! ");
            }
            catch(Exception ex)
            {
                _log.LogError(ex,$"Can not binding queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! reason: {ex.Message}");
                throw new InvalidOperationException($"Can not binding queue(name: {_receiveQueue}, exchange: {_receiveExchange}), routingkey: {_routingKey} at rabbit mq! reason: {ex.Message}");
            }
        }

    }
}
