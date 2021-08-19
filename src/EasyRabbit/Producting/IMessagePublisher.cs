using System;
using System.Collections.Generic;
using EasyRabbit.Options;

namespace EasyRabbit.Producting
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message, IDictionary<string, object> headers = null, PublishOptions publishOptions = null, ServerOptions serverOptions = null, Func<T, ReadOnlyMemory<byte>> serialize = null);
    }
}
