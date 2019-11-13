using System;
using System.Threading.Tasks;
using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Serilog;
using Serilog.AspNetCore;
using Atlantis.Rabbit.Models;

namespace Atlantis.Rabbit.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder().ConfigureLogging((hostContext, configLogging) =>
            {
                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
                configLogging.AddSerilog();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory, SerilogLoggerFactory>();
                services.AddScoped<TestHandler>();
                services.AddRabbit(builder =>
                {
                    builder.ServerOptions = new RabbitServerSetting()
                    {
                        Host = "127.0.0.1",
                        UserName = "guest",
                        Password = "guest",
                        Port = 5672,
                        VirtualHost = "/"
                    };
                    builder.CreateConsumer<Test>().SetName("test").ConfigHandler<TestHandler>().AddBinding("test").Build();
                    builder.CreateProducer<Test>().SetName("test").SetType(ExchangeType.Fanout).Build();
                });
            });
            var host = hostBuilder.Build();
            host.Services.UseRabbit();

            var producer = host.Services.GetService<Producer<Test>>();
            var nowait = Task.Run(() =>
            {
                Task.Delay(1000);
                producer.Publish(new Test() { Name = "Hello rabbit" });
            });

            await host.RunAsync();
        }
    }

    public class Test
    {
        public string Name { get; set; }
    }

    public class TestHandler : Atlantis.Rabbit.MessagingHandler<Test>
    {
        protected override Task Process(Test message, ConsumerMessagingContext<Test> context)
        {
            Console.WriteLine(message.Name);
            return Task.CompletedTask;
        }
    }
}
