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

namespace Atlantis.Rabbit.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureLogging((hostContext, configLogging) =>
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
                    services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory,SerilogLoggerFactory>();
                    services.AddRabbit(builder=>
                    {
                        builder.ServerOptions=new RabbitServerSetting()
                        {
                            Host="120.77.144.4",
                            UserName="admin",
                            Password="1qa@WS3ed",
                            Port=-1,
                            VirtualHost="dev"
                        };
                        builder.ScanAssemblies=new string[]{typeof(Program).Assembly.FullName,typeof(Program).Assembly.FullName};
                    });
                    var message=new AddWaterMarkDto(){FileToken="11",WaterMarkText="22"};
                    var headers=new Dictionary<string,object>();
                    headers.Add("TTL",864000000);
                    var serviceProvider=services.BuildServiceProvider().UseRabbit();

                    var publisher=serviceProvider.GetService<IRabbitMessagePublisher>();
                    publisher.Publish(message,headers);
                })
                .RunConsoleAsync();
            
        }
    }


    [RabbitPublishTo(Exchange="cap.default.router",RoutingKey="image.water.mark")]
    public class AddWaterMarkDto
    {
        /// <summary>
        /// 文件token
        /// </summary>
        public string FileToken { get; set; }

        /// <summary>
        /// 水印文字
        /// </summary>
        public string WaterMarkText { get; set; }

    }

    public class WaterMarkHandler : RabbitMQMessagingHandler<AddWaterMarkDto>
    {
        public WaterMarkHandler()
        {
        }

        public override string Queue => "fileservice.v1";

        public override string Exchange => "cap.default.router";

        public override string VirtualHost=>"dev";

        public override string RoutingKey=>"image.water.mark";

        public override long TTL=>864000000;

        protected override Task Handle(AddWaterMarkDto message)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));
            return Task.CompletedTask;
        }
    }

     public class ConvertVideoHandler
        : RabbitMQMessagingHandler<AddWaterMarkDto>
    {
        public ConvertVideoHandler()
        {}

        public override string Queue => "fileservice.convertvideo";

        public override string Exchange => "fileservice.convertvideo.exchange";

        protected override async Task Handle(AddWaterMarkDto message)
        {
            throw new NotImplementedException();
        }
    }
}
