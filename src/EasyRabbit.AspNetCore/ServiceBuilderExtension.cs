using System;
using EasyRabbit;
using EasyRabbit.AspNetCore;
using EasyRabbit.Basic;
using EasyRabbit.Producting;
using EasyRabbit.Publishes;
using EasyRabbit.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuilder
    {
        public static IServiceCollection AddEasyRabbit(this IServiceCollection services, Action<RabbitMQBuilder> builderAction)
        {
            var builder = new RabbitMQBuilder();
            builderAction(builder);

            services.AddLogging();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<RabbitMQBuilder>(builder);
            services.AddSingleton<ILoggerFactory, MicrosoftLoggerFactory>();
            services.AddSingleton<IObjectContainer, ServiceProviderObjectContainer>();
            services.AddSingleton<IRabbitMQQueueInfomation, RabbitMQQueueInfomation>();
            builder.UseNewtonSoftJson();

            foreach (var consumeHandler in builder.ConsumeBuilders)
                services.AddScoped(consumeHandler.HandlerType);

            return services;
        }
    }
}
