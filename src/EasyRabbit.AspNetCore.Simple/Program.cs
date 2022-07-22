using System;
using EasyRabbit.AspNetCore.Test.MQMessages;
using EasyRabbit.Options;
using EasyRabbit.Producting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using EasyRabbit.Basic;

namespace EasyRabbit.AspNetCore.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .AddEasyRabbit((builder, context, services) =>
                {
                    builder.AddGlobalServerOptions(new ServerOptions()
                    {
                        Host = "staging-mq01.fnlinker.com",
                        Port = 5672,
                        UserName = "admin",
                        Password = "1qa@WS3ed",
                        VirtualHost = "staging"
                    });
                    // builder.AddConsumer().AddHandler<HelloHandler>().UseConsumeOptions(new ConsumeOptions()
                    // {
                    //     Queue = "hello",
                    //     Exchange = "hello",
                    //     RoutingKey = "hello"
                    // });
                    // builder.AddProducer().AddMessage<HelloMessage>().UsePublishOptions(new PublishOptions()
                    // {
                    //     Exchange = "hello",
                    //     RoutingKey = "hello"
                    // });
                })
                .Build()
                .UseEasyRabbit();

            var queueInfomation = host.Services.GetService<IRabbitMQQueueInfomation>();
            // var publisher = host.Services.GetService<IMessagePublisher>();
            // publisher.Publish(new HelloMessage() { Name = "test" });
            Console.WriteLine(queueInfomation.GetMessageCount("businessQueueA"));
        }
    }
}
