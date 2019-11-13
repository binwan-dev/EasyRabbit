using EasyRabbit.Models;
using RabbitMQ.Client.Events;

namespace EasyRabbit
{
    public class ConsumerMessagingContext<TMessaging>
    {
        public Consumer<TMessaging> Consumer { get; set; }

        public BasicDeliverEventArgs Properties { get; set; }
    }
}
