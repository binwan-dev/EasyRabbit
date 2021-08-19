using System;
using System.Collections.Generic;
using System.Text;
using EasyRabbit.Options;

namespace EasyRabbit.Producting
{
    public class PublishMessagingMetadata
    {
        public PublishMessagingMetadata(Type messageType, ServerOptions serverOptions, PublishOptions publishOptions)
        {
            Name = messageType.FullName;
            ServerOptions = serverOptions;
            PublishOptions = publishOptions;
            MessageType = messageType;
        }

        public string Name { get; set; }

        public ServerOptions ServerOptions { get; set; }

        public PublishOptions PublishOptions { get; set; }

        public Type MessageType { get; set; }
    }
}
