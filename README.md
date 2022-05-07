EasyRabbit
======================  
该项目是用来简化RabbitMQ访问，提供一种Publish/Consume 方式访问  
### 1. 安装  
``` bash
dotnet add package EasyRabbit.AspNetCore
```
### 2. 在 Program 注册
代码引用举例（如果使用Attribute,则无需每个注册）

> 使用 IServiceCollection 注册（常用于 AspNetCore WebApi项目中）
``` csharp
// Services or Use hostBuilder.AddEasyRabbit()
services.AddEasyRabbit((builder)=>
{
    builder.AddGlobalServerOptions(new ServerOptions()
    {
        Host = "0.0.0.0",
        Port = 6572,
        UserName = "user",
        Password = "password",
        VirtualHost = "virtualhost"
    });
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
> 使用 IHostBuilder 方式注册（常用于控制台程序中）
``` csharp
Host.CreateDefaultBuilder()
    .AddEasyRabbit((builder, services, context) =>
    {
        builder.AddGlobalServerOptions(new ServerOptions()
        {
            Host = "0.0.0.0",
            Port = 6572,
            UserName = "user",
            Password = "password",
            VirtualHost = "virtualhost"
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

```
### 3. 消费或生产消息
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
var publisher=servicers.GetService<IMessagePublisher>();
publisher.Publish(new HelloMessage());
```
