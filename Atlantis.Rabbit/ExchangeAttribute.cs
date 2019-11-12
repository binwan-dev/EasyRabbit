using System;
using System.Collections.Generic;

namespace Atlantis.Rabbit
{
    public class ExchangeAttribute : Attribute
    {
        public ExchangeAttribute()
        {
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, string> Arguments { get; set; }

        public string RoutingKey { get; set; }

        public string VirtualHost { get; set; }
    }
}
