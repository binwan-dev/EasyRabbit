using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Atlantis.Rabbit
{
    public interface IRabbitMessagingHandler
    {
        string Queue { get; }

        string Exchange { get; }

        string VirtualHost { get; }

        string RoutingKey{get;}

        bool IsEnable { get; }

        string Name{get;}

        long TTL{get;}

        Task Handle(RabbitConnection connection, object model, BasicDeliverEventArgs e);
    }
}
