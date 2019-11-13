using System;
using System.Collections.Generic;

namespace EasyRabbit
{
    public class RabbitConnectionPool:IDisposable
    {
        private readonly IDictionary<string, RabbitConnection> _connectionDic;

        public RabbitConnectionPool()
        {
            _connectionDic=new Dictionary<string, RabbitConnection>();
        }

        public void Dispose()
        {
            foreach(var item in _connectionDic)
            {
                item.Value.BaseConnection.Dispose();
            }
            _connectionDic.Clear();
        }

        public RabbitConnection GetConnection(RabbitServerSetting setting, string virtualHost = "")
        {
            if (!string.IsNullOrWhiteSpace(virtualHost))
            {
                setting.VirtualHost = virtualHost;
            }

            var key = $"{setting.Host}{setting.Port}{setting.VirtualHost}";
            if (!_connectionDic.TryGetValue(key, out RabbitConnection value))
            {
                value = new RabbitConnection(setting);
                _connectionDic.Add(key, value);
            }
            return value;
        }

        
    }
}
