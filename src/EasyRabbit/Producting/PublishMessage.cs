using System;
using System.Collections.Generic;
using EasyRabbit.Options;

namespace EasyRabbit.Producting
{
    public class PublishMessage
    {
        public PublishMessage(ServerOptions serverOptions, ReadOnlyMemory<byte> body, PublishOptions publishOptions)
        {
            ServerOptions = serverOptions;
            Body = body;
            PublishOptions = publishOptions;
            Headers = new Dictionary<string, object>();
        }

        public ServerOptions ServerOptions { get; }

        public ReadOnlyMemory<byte> Body { get; }

        public IDictionary<string, object> Headers { get; }

        public PublishOptions PublishOptions { get; }
    }
}