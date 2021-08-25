using System;
using EasyRabbit.Options;

namespace EasyRabbit.Producting
{
    public class PublishBuilder
    {
        public PublishBuilder()
        {
            ServerOptions = ServerOptions.Default;
        }

        public PublishOptions PublishOptions { get; private set; }

        public ServerOptions ServerOptions { get; private set; }

        public Type MessageType { get; private set; }

        public PublishBuilder UsePublishOptions(PublishOptions publishOptions)
        {
            PublishOptions = publishOptions;
            return this;
        }

        public PublishBuilder UseServerOptions(ServerOptions serverOptions)
        {
            ServerOptions = serverOptions;
            return this;
        }

        public PublishBuilder AddMessage<T>()
        {
            MessageType = typeof(T);
            return this;
        }
    }
}