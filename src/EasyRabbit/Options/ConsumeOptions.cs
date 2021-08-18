namespace EasyRabbit.Options
{
    public class ConsumeOptions
    {
        public ConsumeOptions()
        {
            IsAutoAck = true;
            IsAutoAckWhenDeserializeFailed = true;
        }

        public string VirtualHost { get; set; }

        public string RoutingKey { get; set; }

        public string Queue { get; set; }

        public string Exchange { get; set; }

        /// <summary>
        /// Default true
        /// </summary>
        public bool IsAutoAckWhenDeserializeFailed { get; set; }

        /// <summary>
        /// Default true
        /// </summary>
        public bool IsAutoAck { get; set; }

        public ushort PrefetchCount { get; set; }
    }
}