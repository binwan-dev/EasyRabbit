using System;
using System.Threading.Tasks;
using EasyRabbit.AspNetCore.Test.MQMessages;
using EasyRabbit.Consuming;

namespace EasyRabbit.AspNetCore.Test
{
    public class HelloHandler : ConsumeMessagingHandler<HelloMessage>
    {
        protected override Task HandleAsync(IConsumeMessagingContext<HelloMessage> message)
        {
            Console.WriteLine($"Hello {message.Data.Name}");
            return Task.CompletedTask;
        }
    }
}