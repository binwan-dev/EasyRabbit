using System;
using EasyRabbit.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace EasyRabbit.AspNetCore
{
    public class ServiceProviderObjectContainer : IObjectContainer
    {
        public static IServiceProvider Provider;
        private readonly IServiceScope _serviceScope;

        public ServiceProviderObjectContainer()
        {
        }

        public ServiceProviderObjectContainer(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        private IServiceProvider provider => _serviceScope?.ServiceProvider ?? Provider;

        public IObjectContainer CreateScope()
        {
            return new ServiceProviderObjectContainer(provider.CreateScope());
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
        }

        public T Resolve<T>(Type type = null)
        {
            if (type != null)
                return (T)provider.GetService(type);

            return provider.GetService<T>();
        }
    }
}
