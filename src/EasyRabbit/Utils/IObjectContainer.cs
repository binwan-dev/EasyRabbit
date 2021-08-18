using System;

namespace EasyRabbit.Utils
{
    public interface IObjectContainer
    {
        T Resolve<T>(Type type = null);
    }
}