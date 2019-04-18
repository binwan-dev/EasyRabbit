namespace Atlantis.Rabbit
{
    public interface IRabbitMessagePublisher
    {
        void Publish<T>(T message);
    }     
}
