using Atlantis.Rabbit.Models;

namespace Atlantis.Rabbit
{
    internal interface IConsumer
    {
        QueueOptions Queue { get; }

        void TryBind(RabbitConnection connection);
    }
}
