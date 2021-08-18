using System;

namespace EasyRabbit.Utils
{
    public class DefaultObjectContainer : IObjectContainer
    {
        public T Resolve<T>(Type type = null)
        {
            throw new NotImplementedException();
        }
    }
}