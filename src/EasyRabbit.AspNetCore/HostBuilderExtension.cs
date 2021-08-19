using System;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EasyRabbit.AspNetCore
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder AddEasyRabbit(this IHostBuilder builder, Action<RabbitMQBuilder> builderAction)
        {
            builder.ConfigureServices((b, services) =>
            {
                var rabbitMQBuilder = new RabbitMQBuilder();
                builderAction(rabbitMQBuilder);
                services.AddLogging();
                services.AddSingleton<RabbitMQBuilder>(rabbitMQBuilder);
                services.AddSingleton<ILoggerFactory, MicrosoftLoggerFactory>();
                services.AddSingleton<IObjectContainer, ServiceProviderObjectContainer>();
                rabbitMQBuilder.UseNewtonSoftJson();
            });
            return builder;
        }
    }
}