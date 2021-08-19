using System;
using EasyRabbit;
using EasyRabbit.AspNetCore;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder AddEasyRabbit(this IHostBuilder builder, Action<RabbitMQBuilder, HostBuilderContext, IServiceCollection> builderAction)
        {
            builder.ConfigureServices((context, services) =>
            {
                var rabbitMQBuilder = new RabbitMQBuilder();
                builderAction(rabbitMQBuilder, context, services);
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