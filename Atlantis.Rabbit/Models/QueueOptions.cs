using System.Collections.Generic;

namespace Atlantis.Rabbit.Models
{
    public class QueueOptions
    {
        public QueueOptions()
        {
            Arguments = new Dictionary<string, object>();
            Bindings = new List<QueueBindingOptions>();
            RoutingKey = "";
        }

        public string Name { get; set; }

        public string VirtualHost { get; set; }

        public string RoutingKey { get; set; }

        public bool Exclusive { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Arguments { get; set; }

        public bool IsEnable { get; set; }

        public IList<QueueBindingOptions> Bindings { get; set; }
    }
}
