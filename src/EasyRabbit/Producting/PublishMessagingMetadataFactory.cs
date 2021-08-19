using System;
using System.Collections.Generic;

namespace EasyRabbit.Producting
{
    public static class PublishMessagingMetadataFactory
    {
        private static readonly IDictionary<string, PublishMessagingMetadata> _metadataDic;

        static PublishMessagingMetadataFactory()
        {
            _metadataDic = new Dictionary<string, PublishMessagingMetadata>();
        }

        public static void AddMessagingMetadata(PublishMessagingMetadata metadata)
        {
            _metadataDic.Add(metadata.MessageType.FullName, metadata);
        }

        public static PublishMessagingMetadata GetMetadata(Type messageType)
        {
            if (!_metadataDic.TryGetValue(messageType.FullName, out PublishMessagingMetadata metadata))
                throw new KeyNotFoundException(messageType.FullName);
            return metadata;
        }
    }
}