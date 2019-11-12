using Atlantis.Rabbit.Models;
using RabbitMQ.Client.Events;

namespace Atlantis.Rabbit
{
    public class ConsumerMessagingContext<TMessaging>
    {
        public Consumer<TMessaging> Consumer { get; set; }

        public BasicDeliverEventArgs Properties { get; set; }
    }
}
