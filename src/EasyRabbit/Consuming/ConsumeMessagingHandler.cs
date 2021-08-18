using System;
using System.Threading.Tasks;
using EasyRabbit.Utils;
using RabbitMQ.Client.Events;

namespace EasyRabbit.Consuming
{
    public abstract class ConsumeMessagingHandler<TMessage> : IConsumeMessagingHandler<TMessage> where TMessage : class
    {
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerialzer;
        private readonly ConsumeMetadata _metadata;

        public ConsumeMessagingHandler()
        {
            _logger = ObjectContainerFactory.ObjectContainer.Resolve<ILoggerFactory>().CreateLogger(this.GetType());
            _jsonSerialzer = ObjectContainerFactory.ObjectContainer.Resolve<IJsonSerializer>();
            _metadata = ConsumeMetadataFactory.Get(this.GetType().FullName);
        }

        public async Task HandleAsync(ConsumeChannel channel, object model, BasicDeliverEventArgs e)
        {
            ConsumeMessagingContext<TMessage> messagingContext = null;
            try
            {
                messagingContext = new ConsumeMessagingContext<TMessage>(channel, model, e, decodeMessage, _metadata);

                _logger.Debug("The Queue({0}) receive message! Message Data: {1}", _metadata.ConsumeOptions.Queue, messagingContext.Data);
                await HandleAsync(messagingContext);
                _logger.Debug("The Queue({0}) handle complete! Message Data: {1}", _metadata.ConsumeOptions.Queue, messagingContext.Data);

                if (_metadata.ConsumeOptions.IsAutoAck)
                    messagingContext.Ack();
            }
            catch (DeserializeException ex)
            {
                HandleDeserializeException(ex, messagingContext);
            }
            catch (Exception ex)
            {
                HandleUnknowException(ex, messagingContext);
            }
        }

        private TMessage decodeMessage(ReadOnlyMemory<byte> body)
        {
            try
            {
                return Deserialize(body);
            }
            catch (Exception ex)
            {
                throw new DeserializeException("Deserialize message failed!", ex);
            }
        }

        protected abstract Task HandleAsync(IConsumeMessagingContext<TMessage> message);

        protected virtual (bool IsContinue, IConsumeMessagingContext<TMessage> MessageContext) BeforeHandle(IConsumeMessagingContext<TMessage> messageContext) => (true, messageContext);

        protected virtual (bool IsContinue, IConsumeMessagingContext<TMessage> MessageContext) AfterHandle(IConsumeMessagingContext<TMessage> messageContext) => (true, messageContext);

        protected virtual TMessage Deserialize(ReadOnlyMemory<byte> messageBody)
        {
            return _jsonSerialzer.Deserialize<TMessage>(messageBody);
        }

        protected virtual void HandleDeserializeException(DeserializeException ex, IConsumeMessagingContext<TMessage> messagingContext)
        {
            if (_metadata.ConsumeOptions.IsAutoAckWhenDeserializeFailed)
            {
                _logger.Error(ex.InnerException, "The Queue({0}) deserialize message failed! Will auto ack this message!", _metadata.ConsumeOptions.Queue);
                messagingContext.Ack();
            }
            else
            {
                _logger.Error(ex.InnerException, "The Queue({0}) deserialize message failed! Will reject message!", _metadata.ConsumeOptions.Queue);
                messagingContext.Reject();
            }
        }

        protected virtual void HandleUnknowException(Exception ex, IConsumeMessagingContext<TMessage> messagingContext)
        {
            _logger.Error(ex, "The Queue({0}) deserialize message failed! Will reject message!", _metadata.ConsumeOptions.Queue);
            messagingContext.Reject();
        }
    }
}
