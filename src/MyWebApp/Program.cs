using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using MyNuget.Azure.AppConfiguration;
using MyWebApp;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyApi();

builder.Services.AddAzureClients(clientFactoryBuilder =>
{
    clientFactoryBuilder.AddServiceBusClient("sdx-olympe-sbx.servicebus.windows.net");

    clientFactoryBuilder
        .AddClient<ServiceBusSender, ServiceBusClientOptions>(
            (_, _, provider) => provider
                .GetRequiredService<ServiceBusClient>()
                .CreateSender("my-topic")
            )
        .WithName("MyServiceBusTopicSender");



    // This will register the EventHubProducerClient using the default credential.
    clientFactoryBuilder.AddEventHubProducerClientWithNamespace("oss-notifications.servicebus.windows.net", "olympe -core").WithName("MyEventHubProducerClient");

    // By default, DefaultAzureCredential is used, which is likely desired for most
    // scenarios. If you need to restrict to a specific credential instance, you could
    // register that instance as the default credential instead.
    clientFactoryBuilder.UseCredential(new ManagedIdentityCredential());
});

builder.AddTodos();
builder.AddMongoTodos();

//builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddHttpClient<SelfHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5555");
});

var app = builder.Build();

var myAppConfigurationOptions = new MyAppConfigurationOptions();
builder.Configuration.Bind(MyAppConfigurationOptions.SectionName, myAppConfigurationOptions);
if (myAppConfigurationOptions.Endpoint?.IsAbsoluteUri ?? false)
{
    app.UseAzureAppConfiguration();
}

app.MapMyApi();

app.Run();
