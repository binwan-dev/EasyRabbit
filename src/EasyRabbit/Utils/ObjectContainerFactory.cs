using System;

namespace EasyRabbit.Utils
{
    public class ObjectContainerFactory
    {
        private static IObjectContainer _container;

        public static void RegisterContainer(IObjectContainer container)
        {
            _container = container;
        }

        public static IObjectContainer ObjectContainer => _container;
    }
}