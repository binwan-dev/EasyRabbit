using EasyRabbit.Extensions.Newtonsoft.Json;
using EasyRabbit.Utils;

namespace EasyRabbit
{
    public static class RabbitMQBuilderExtension
    {
        public static RabbitMQBuilder UseNewtonSoftJson(this RabbitMQBuilder builder)
        {
            EasyRabbit.Utils.SerializeFactory.RegisterSerializer(new JsonSerializer());
            return builder;
        }
    }
}