namespace Atlantis.Rabbit.Models
{
    public class QosOptions
    {
        public uint PrefetchSize { get; set; }

        public ushort PrefetchCount { get; set; }

        public bool Global { get; set; }

        public static QosOptions Default()
        {
            return new QosOptions()
            {
                PrefetchCount=1,
                PrefetchSize=0,
                Global=false
            };
        }
    }
}
