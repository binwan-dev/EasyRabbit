using System;

namespace EasyRabbit
{
    public interface IPublishUnitOfWork:IDisposable
    {
        void Publish(Action publishAction);
        
        void Commit();
    }
}
