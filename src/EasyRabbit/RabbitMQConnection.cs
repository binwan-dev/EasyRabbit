using System;
using System.Threading;
using EasyRabbit.Options;
using EasyRabbit.Utils;
using RabbitMQ.Client;

namespace EasyRabbit
{
    public class RabbitMQConnection
    {
        private readonly ServerOptions _serverOptions;
        private readonly string _virtualHost;
        public event Action<RabbitMQConnection> Connected;
        private readonly ILogger _logger;
        private IConnection _connection;
        private int _reconnectMilliSeconds = 600;
        private int _reconnectTimes = 1;
        private static int _connecting = 0;

        public RabbitMQConnection(ServerOptions options, string virtualHost)
        {
            _serverOptions = options;
            _virtualHost = virtualHost;
            _logger = ObjectContainerFactory.ObjectContainer.Resolve<ILoggerFactory>().CreateLogger<RabbitMQConnection>();
        }

        public IConnection Connection => _connection;

        public void Connect(bool isAutoConnect = true)
        {
            try
            {
                _connection = RabbitMQUtils.CreateNewConnection(_serverOptions, _virtualHost);
                if (Connected != null)
                    Connected.Invoke(this);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The rabbit mq service cannot connect! ServerOptions: {0}", _serverOptions.ToString());
                if (isAutoConnect)
                {
                    _logger.Info($"Auto connect is enable, the service will reconnect to the rabbit mq server after {_reconnectMilliSeconds} ms!");
                    tryReConnect();
                }
                else
                {
                    throw ex;
                }
            }
        }

        #region Reconnect
        private void tryReConnect()
        {
            if (!enterReConnect()) return;
            Thread.Sleep(_reconnectMilliSeconds);
            try
            {
                _logger.Info("Try reconnect to server! server info: ip={0}, port={1}, virtual host={2}, username={3}", _serverOptions.Host, _serverOptions.Port, _virtualHost, _serverOptions.UserName);
                reConnect();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The connection connect failed! ServerOptions: {0}", _serverOptions.ToString());
                exitReConnect();
                var sleepingTime = _reconnectMilliSeconds * _reconnectTimes * _reconnectTimes;
                _logger.Wran($"The connection reconnect to server after {sleepingTime / 1000} s!");
                Thread.Sleep(sleepingTime);
                if (_reconnectTimes <= 30) _reconnectTimes++;
                tryReConnect();
                return;
            }
            exitReConnect();
        }

        private void reConnect()
        {
            _connection = RabbitMQUtils.CreateNewConnection(_serverOptions, _virtualHost);
            if (Connected != null)
                Connected(this);
            _reconnectTimes = 1;
        }

        private bool enterReConnect()
        {
            return Interlocked.CompareExchange(ref _connecting, 1, 0) == 0;
        }

        private void exitReConnect()
        {
            Interlocked.Exchange(ref _connecting, 0);
        }
        #endregion


    }
}