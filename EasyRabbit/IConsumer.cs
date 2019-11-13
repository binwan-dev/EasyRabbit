using EasyRabbit.Models;

namespace EasyRabbit
{
    internal interface IConsumer
    {
        QueueOptions Queue { get; }

        void TryBind(RabbitConnection connection);
    }
}
