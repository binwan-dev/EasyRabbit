using System;
using System.Collections.Concurrent;

namespace Atlantis.Rabbit
{
    public class PublishUnitOfWork : IPublishUnitOfWork
    {
        private ConcurrentQueue<Action> _publishQueue;

        public PublishUnitOfWork()
        {
            _publishQueue=new ConcurrentQueue<Action>();
        }
        
        public void Commit()
        {
            while(true)
            {
                if(!_publishQueue.TryDequeue(out Action result))
                {
                    if(_publishQueue.Count==0)
                    {
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("Get publish action in the publishqueue failed!");
                    }
                }
                result();
            }
        }

        public void Dispose()
        {
            _publishQueue=null;
        }

        public void Publish(Action publishAction)
        {
            _publishQueue.Enqueue(publishAction);
        }
    }
}
