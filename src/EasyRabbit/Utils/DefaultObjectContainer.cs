using System;

namespace EasyRabbit.Utils
{
    public class DefaultObjectContainer : IObjectContainer
    {
        public IObjectContainer CreateScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(Type type = null)
        {
            throw new NotImplementedException();
        }
    }
}