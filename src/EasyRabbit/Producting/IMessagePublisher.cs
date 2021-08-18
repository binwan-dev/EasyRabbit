namespace EasyRabbit.Producting
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message);
    }
}
