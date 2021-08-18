using System;
using System.Collections.Generic;
using EasyRabbit.Options;

namespace EasyRabbit.Consuming
{
    public class ConsumeMetadata
    {
        public ServerOptions ServerOptions { get; set; }

        public ConsumeOptions ConsumeOptions { get; set; }

        public Type HandlerType { get; set; }
    }

    public class ConsumeMetadataFactory
    {
        private static readonly IDictionary<string, ConsumeMetadata> _metadataDic;

        public static ConsumeMetadata Get(string handlerName)
        {
            if (!_metadataDic.TryGetValue(handlerName, out ConsumeMetadata metadata))
                throw new KeyNotFoundException($"NotFound metadata at the key({handlerName})");

            return metadata;
        }
    }
}