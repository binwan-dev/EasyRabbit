namespace EasyRabbit.Options
{
    public class PublishOptions
    {
        public string Exchange { get; set; }

        public string VirtualHost { get; set; }

        public string RoutingKey { get; set; }
    }
}