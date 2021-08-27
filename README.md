EasyRabbit
======================  
该项目是用来简化RabbitMQ访问，提供一种Publish/Consume 方式访问
使用  
``` csharp
dotnet add package EasyRabbit.AspNetCore
```
代码引用举例（如果使用Attribute,则无需每个注册）
``` csharp
// Services or Use hostBuilder.AddEasyRabbit()
services.AddEasyRabbit((builder,context,services)=>
{
    builder.AddGlobalServerOptions(context.Configuration.GetSection("RabbitServerOptions").Get<ServerOptions>());
    builder.AddConsumer().AddHandler<HelloHandler>().UseConsumeOptions(new ConsumeOptions()
    {
        Queue = $"queue",
        Exchange = "exchange",
        RoutingKey = $"routingkey",
    });
    builder.AddProducer().AddMessage<HelloMessage>().UsePublishOptions(new PublishOptions()
    {
        Exchange = "excahge",
        RoutingKey = "routingkey",
    });
})

// ApplicationBuilder Or Use Host.UseEasyRabbit();
applicationBuilder.UseRabbit()
```

``` csharp
# Consume
public class HelloHandler:ConsumeMessagingHandler<HelloMessage>
{
    public Task HandleAsync(ConsumeMessagingContext<HelloMessage> message)
    {
        throw new NotImplementException();
    }
}
# Producer
var publisher=new MessagePublisher();
publisher.Publish(new HelloMessage());
```
