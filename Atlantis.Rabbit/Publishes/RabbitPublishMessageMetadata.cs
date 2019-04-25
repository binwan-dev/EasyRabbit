namespace Atlantis.Rabbit
{
    public class RabbitPublishMessageMetadata
    {
        public RabbitPublishMessageMetadata(RabbitPublishToAttribute attribute, RabbitServerSetting connectHostString)
        {
            ConnectSetting = connectHostString;
            Exchange = attribute.Exchange;
            SerializeType = attribute.SerializeType;
            RoutingKey = attribute.RoutingKey;
        }

        public RabbitServerSetting ConnectSetting { get; set; }

        public string Exchange { get; set; }

        public string RoutingKey { get; set; }

        public RabbitPublishToAttribute Attribute{get;set;}

        public PublishMessageSerialzeType SerializeType { get; set; }
    }
}
