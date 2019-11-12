using System.Collections.Generic;

namespace Atlantis.Rabbit.Models
{
    public class QueueBindingOptions
    {
        public QueueBindingOptions()
        {
            Arguments = new Dictionary<string, object>();
        }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> Arguments { get; set; }
    }
}
