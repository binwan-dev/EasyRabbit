using System;

namespace Atlantis.Rabbit
{
    public interface IPublishUnitOfWork:IDisposable
    {
        void Publish(Action publishAction);
        
        void Commit();
    }
}
