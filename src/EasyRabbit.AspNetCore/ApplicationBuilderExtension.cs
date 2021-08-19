using EasyRabbit;
using EasyRabbit.AspNetCore;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseEasyRabbit(this IApplicationBuilder applicationBuilder)
        {
            ServiceProviderObjectContainer.Provider = applicationBuilder.ApplicationServices;
            var objectaContainer = applicationBuilder.ApplicationServices.GetService<IObjectContainer>();
            var builder = applicationBuilder.ApplicationServices.GetService<RabbitMQBuilder>();
            builder.RegisterObjectContainer(objectaContainer);
            builder.Start();

            return applicationBuilder;
        }
    }
}