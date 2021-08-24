using System;
using EasyRabbit;
using EasyRabbit.AspNetCore;
using EasyRabbit.Extensions.Newtonsoft.Json;
using EasyRabbit.Producting;
using EasyRabbit.Publishes;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBuilder
    {
        public static IServiceCollection AddEasyRabbit(this IServiceCollection services, Action<RabbitMQBuilder> builderAction)
        {
            var builder = new RabbitMQBuilder();
            builderAction(builder);
            services.RegisterRabbit(builder);
            return services;
        }

        internal static IServiceCollection RegisterRabbit(this IServiceCollection services, RabbitMQBuilder builder)
        {
            services.AddLogging();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<RabbitMQBuilder>(builder);
            services.AddSingleton<ILoggerFactory, MicrosoftLoggerFactory>();
            services.AddSingleton<IObjectContainer, ServiceProviderObjectContainer>();
            builder.UseNewtonSoftJson();
            return services;
        }
    }
}