using Azure.Messaging.ServiceBus;
using MyNuget.Azure.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceBusWebApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddBusSender<TBusSenderOptions>(this IHostApplicationBuilder builder)
        where TBusSenderOptions : class, IBusSenderOptions
    {
        builder.Services.AddOptionsWithValidateOnStart<AzureServiceBusOptions>()
            .BindConfiguration(AzureServiceBusOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), $"The configuration key {AzureServiceBusOptions.SectionName}:{nameof(AzureServiceBusOptions.ConnectionString)} cannot be null")
            ;

        builder.Services.AddOptionsWithValidateOnStart<TBusSenderOptions>()
            .BindConfiguration(TBusSenderOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(TBusSenderOptions.ClientName), $"The value {nameof(TBusSenderOptions.ClientName)} for the type {typeof(TBusSenderOptions).FullName} cannot be null")
            .Validate(options => !string.IsNullOrWhiteSpace(options.QueueOrTopicName), $"The configuration key {TBusSenderOptions.SectionName}:{nameof(IBusSenderOptions.QueueOrTopicName)} cannot be null")
            .Validate(options => !options.QueueOrTopicName.Contains('+'), $"The configuration key {TBusSenderOptions.SectionName}:{nameof(IBusSenderOptions.QueueOrTopicName)} cannot contains '+' character")
            .Validate(options => options.QueueOrTopicName.Length <= 50, $"The configuration key {TBusSenderOptions.SectionName}:{nameof(IBusSenderOptions.QueueOrTopicName)} cannot exceed 50 characters")
            ;

        builder.Services.AddAzureClients(clientFactoryBuilder =>
        {
            clientFactoryBuilder.AddServiceBusClient(builder.Configuration.GetSection(AzureServiceBusOptions.SectionName));

            clientFactoryBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>(
            (_, _, provider) =>
            {
                var serviceBusClient = provider.GetRequiredService<ServiceBusClient>();
                var options = provider.GetRequiredService<IOptions<TBusSenderOptions>>().Value;

                return serviceBusClient.CreateSender(options.QueueOrTopicName);
            })
            .WithName(TBusSenderOptions.ClientName);
        });

        return builder;
    }
}
