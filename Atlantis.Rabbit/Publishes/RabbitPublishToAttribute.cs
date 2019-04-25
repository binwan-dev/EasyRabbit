using System;
using System.Collections.Generic;

namespace Atlantis.Rabbit
{
    public class RabbitPublishToAttribute : Attribute
    {
        public RabbitPublishToAttribute()
        {
            SerializeType=PublishMessageSerialzeType.Json;
            Headers=new Dictionary<string,object>();
        }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public string VirtualHost{get;set;}

        public IDictionary<string,object> Headers{get;set;}

        public PublishMessageSerialzeType SerializeType { get; set; }
    }

    public enum PublishMessageSerialzeType
    {
        Proto = 0,
        Json = 1
    }
}
