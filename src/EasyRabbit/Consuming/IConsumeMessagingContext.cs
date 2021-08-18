namespace EasyRabbit.Consuming
{
    public interface IConsumeMessagingContext<TMessage>
    {
        TMessage Data { get; }

        void Ack();

        void Nack(bool requeue = true);

        void Reject(bool requeue = true);
    }
}