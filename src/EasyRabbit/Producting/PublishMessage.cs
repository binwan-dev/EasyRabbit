using System;
using System.Collections.Generic;
using EasyRabbit.Options;

namespace EasyRabbit.Producting
{
    public class PublishMessage
    {
        public ServerOptions ServerOptions { get; }

        public ReadOnlyMemory<byte> Body { get; }

        public IDictionary<string, object> Headers { get; }


    }
}