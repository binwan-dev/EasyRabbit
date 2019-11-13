using System;
using System.Collections.Generic;

namespace EasyRabbit.Models
{
    public class ExchangeOptions
    {
        public ExchangeOptions()
        {
            RoutingKey = "";
        }
        
        public string Name { get; set; }

        public string Type { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public bool Internal { get; set; }

        public string RoutingKey { get; set; }

        public string VirtualHost { get; set; }

        public bool Mandatory { get; set; }

        public IDictionary<string, object> Arguments { get; set; }

    }
}
