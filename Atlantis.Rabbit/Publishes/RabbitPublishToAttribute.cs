using System;

namespace Atlantis.Rabbit
{
    public class RabbitPublishToAttribute : Attribute
    {
        public RabbitPublishToAttribute(string exchange, string groupName, PublishMessageSerialzeType serializeType, string routingKey = "*") : this(exchange, routingKey, groupName, serializeType)
        { }

        public RabbitPublishToAttribute(string exchange, string groupName, string routingKey = "*") : this(exchange, routingKey, groupName, PublishMessageSerialzeType.Proto)
        { }

        public RabbitPublishToAttribute(string exchange, string routingKey, string groupName, PublishMessageSerialzeType serializeType)
        {
            Exchange = exchange;
            GroupName = groupName;
            SerializeType = serializeType;
            RoutingKey = routingKey;
        }

        public string Exchange { get; set; }

        public string GroupName { get; set; }

        public string RoutingKey { get; set; }

        public PublishMessageSerialzeType SerializeType { get; set; }
    }

    public enum PublishMessageSerialzeType
    {
        Proto = 0,
        Json = 1
    }
}
