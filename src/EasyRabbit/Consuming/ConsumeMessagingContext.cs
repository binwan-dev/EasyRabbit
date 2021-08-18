using System;
using EasyRabbit.Utils;
using RabbitMQ.Client.Events;

namespace EasyRabbit.Consuming
{
    public class ConsumeMessagingContext<TMessage> : IConsumeMessagingContext<TMessage> where TMessage : class
    {
        private readonly Func<ReadOnlyMemory<byte>, TMessage> _deserializeFunc;

        public ConsumeMessagingContext(ConsumeChannel channel, object model, BasicDeliverEventArgs e, Func<ReadOnlyMemory<byte>, TMessage> deserializeFunc, ConsumeMetadata metadata)
        {
            Channel = channel;
            Model = model;
            Args = e;
            Metadata = metadata;

            _deserializeFunc = deserializeFunc;
            Data = _deserializeFunc(e.Body);
        }

        public ConsumeChannel Channel { get; private set; }

        public BasicDeliverEventArgs Args { get; private set; }

        public object Model { get; private set; }

        public TMessage Data { get; private set; }

        public ConsumeMetadata Metadata { get; private set; }

        public void Ack()
        {
            Channel.ReceiveChannel.BasicAck(Args.DeliveryTag, false);
        }

        public void Nack(bool requeue = true)
        {
            Channel.ReceiveChannel.BasicNack(Args.DeliveryTag, false, requeue);
        }

        public void Reject(bool requeue = true)
        {
            Channel.ReceiveChannel.BasicReject(Args.DeliveryTag, requeue);
        }
    }
}