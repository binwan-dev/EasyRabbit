using System;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRabbit.AspNetCore
{
    public class ServiceProviderObjectContainer : IObjectContainer
    {
        public static IServiceProvider Provider;

        public T Resolve<T>(Type type = null)
        {
            if (type != null)
                return (T)Provider.GetService(type);

            return Provider.GetService<T>();
        }
    }
}