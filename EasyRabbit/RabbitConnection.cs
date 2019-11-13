using EasyRabbit.Models;
using EasyRabbit.Utilies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyRabbit
{
    public class RabbitConnection
    {
        private readonly RabbitServerSetting _setting;
        private IConnection _connection;
        private readonly ILogger<RabbitConnection> _log;

        private int _connecting = 0;
        private int _reconnectTimes = 1;

        public RabbitConnection(RabbitServerSetting setting)
        {
            _log = RabbitBuilder.ServiceProvider.GetService<ILogger<RabbitConnection>>();
            _setting = setting;
        }

        public IConnection BaseConnection => _connection;

        public void Connect(bool isAutoConnect = true)
        {
            try
            {
                if (_connection != null && _connection.IsOpen)
                {
                    return;
                }

                _connection = CreateConnection();
                SurveillanceConnect();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"The rabbit mq service cannot connect! reason: {ex.Message}");
                if (isAutoConnect)
                {
                    _log.LogInformation($"Auto connect is enable, the service will reconnect to the rabbit mq server after {_setting.ReconnectTimeMillisecond} ms!");
                    TryReConnect();
                }
            }
        }

        public void Close()
        {
            _connection.Close();
        }

        #region Reconnect
        private void TryReConnect()
        {
            if (!EnterReConnect()) return;
            try
            {
                _log.LogInformation($"Try reconnect to server! server info: ip={_setting.Host}, port={_setting.Port}, virtual host={_setting.VirtualHost}, username={_setting.UserName}, password={_setting.Password}");
                ReConnect();
                Thread.Sleep(_setting.ReconnectTimeMillisecond);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"The connection connect failed! the reason is: {ex.Message}");
                ExitReConnect();
                var sleepingTime = _setting.ReconnectTimeMillisecond * _reconnectTimes * _reconnectTimes;
                _log.LogInformation($"The connection reconnect to server after {sleepingTime / 1000} s!");
                Thread.Sleep(sleepingTime);
                if (_reconnectTimes <= 10) _reconnectTimes++;
                TryReConnect();
                return;
            }
            ExitReConnect();

        }

        private void ReConnect()
        {
            _connection = CreateConnection();
            SurveillanceConnect();
            _reconnectTimes = 1;
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

        private Task SurveillanceConnect()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(_setting.ConnectHeartbeat * 1000);
                    if (!_connection.IsOpen)
                    {
                        TryReConnect();
                        break;
                    }
                }
            });
        }

        private IConnection CreateConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }

            var factory = new ConnectionFactory()
            {
                HostName = _setting.Host,
                UserName = _setting.UserName,
                Password = _setting.Password,
                RequestedHeartbeat = _setting.RequestedHeartbeat,
                AutomaticRecoveryEnabled = true
            };
            if (!string.IsNullOrWhiteSpace(_setting.VirtualHost))
            {
                factory.VirtualHost = _setting.VirtualHost;
            }
            if (_setting.Port > 0)
            {
                factory.Port = _setting.Port;
            }
            return factory.CreateConnection();
        }
    }
}
