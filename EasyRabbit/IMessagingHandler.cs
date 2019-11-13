using System.Threading.Tasks;
using EasyRabbit.Models;
using RabbitMQ.Client.Events;

namespace EasyRabbit
{
    public interface IMessagingHandler<TMessaging>
    {
        Task Handle(TMessaging model, ConsumerMessagingContext<TMessaging> context);
    }
}
