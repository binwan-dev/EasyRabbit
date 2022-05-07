using System;
using EasyRabbit.AspNetCore.Test.MQMessages;
using EasyRabbit.Options;
using EasyRabbit.Producting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

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
                        Host = "192.168.3.253",
                        Port = 6672,
                        UserName = "admin",
                        Password = "123456",
                        VirtualHost = "dev"
                    });
                    builder.AddConsumer().AddHandler<HelloHandler>().UseConsumeOptions(new ConsumeOptions()
                    {
                        Queue = "hello",
                        Exchange = "hello",
                        RoutingKey = "hello"
                    });
                    builder.AddProducer().AddMessage<HelloMessage>().UsePublishOptions(new PublishOptions()
                    {
                        Exchange = "hello",
                        RoutingKey = "hello"
                    });
                })
                .Build()
                .UseEasyRabbit();

            var publisher = host.Services.GetService<IMessagePublisher>();
            publisher.Publish(new HelloMessage() { Name = "test" });
            Thread.Sleep(5000);
            Console.WriteLine("Hello World!");
        }
    }
}
