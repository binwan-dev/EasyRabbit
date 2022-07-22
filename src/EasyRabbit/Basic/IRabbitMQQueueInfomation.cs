using System.Threading.Tasks;
using EasyRabbit.Options;

namespace EasyRabbit.Basic
{
    public interface IRabbitMQQueueInfomation
    {
        uint GetMessageCount(string queue,ServerOptions options=null);
    }
}
