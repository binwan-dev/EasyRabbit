using System;
using EasyRabbit;
using EasyRabbit.AspNetCore;
using EasyRabbit.Extensions.Newtonsoft.Json;
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
            services.AddLogging();
            services.AddSingleton<RabbitMQBuilder>(builder);
            services.AddSingleton<ILoggerFactory, MicrosoftLoggerFactory>();
            services.AddSingleton<IObjectContainer, ServiceProviderObjectContainer>();
            builder.UseNewtonSoftJson();
            return services;
        }
    }
}