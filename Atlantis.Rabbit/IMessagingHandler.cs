using System.Threading.Tasks;
using Atlantis.Rabbit.Models;
using RabbitMQ.Client.Events;

namespace Atlantis.Rabbit
{
    public interface IMessagingHandler<TMessaging>
    {
        Task Handle(TMessaging model, ConsumerMessagingContext<TMessaging> context);
    }
}
