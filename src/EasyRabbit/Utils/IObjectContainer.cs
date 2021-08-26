using System;

namespace EasyRabbit.Utils
{
    public interface IObjectContainer : IDisposable
    {
        IObjectContainer CreateScope();

        T Resolve<T>(Type type = null);
    }
}