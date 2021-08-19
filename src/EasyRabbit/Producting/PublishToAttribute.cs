using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRabbit.Publishes
{
    public class PublishToAttribute : Attribute
    {
        public PublishToAttribute(string exchange, string routingKey, string virtualHost = "")
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            VirtualHost = virtualHost;
        }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string VirtualHost { get; set; }
    }

}
