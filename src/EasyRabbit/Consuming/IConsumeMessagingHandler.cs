using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace EasyRabbit.Consuming
{
    public interface IConsumeMessagingHandler
    {
        Task HandleAsync(ConsumeChannel channel, object model, BasicDeliverEventArgs e);
    }

    public interface IConsumeMessagingHandler<TMessage> : IConsumeMessagingHandler
    {

    }
}
