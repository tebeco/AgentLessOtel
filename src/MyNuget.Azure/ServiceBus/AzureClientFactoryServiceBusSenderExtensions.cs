using Azure.Messaging.ServiceBus;
using MyNuget.Azure.ServiceBus;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Azure;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AzureClientFactoryServiceBusSenderExtensions
{
    public static ServiceBusMessage CreateMessage<TMessage>(this IAzureClientFactory<ServiceBusSender> _, TMessage messageDto)
        where TMessage : notnull
        => _.CreateMessage(messageDto, ServiceBusMessageSerializer.JsonBinary);

    public static ServiceBusMessage CreateMessage<TMessage>(this IAzureClientFactory<ServiceBusSender> _, TMessage messageDto, ServiceBusMessageSerializer serializer)
        where TMessage : notnull
    {
        var servicesBusMessage = new ServiceBusMessage();
        servicesBusMessage.WithBody(messageDto, serializer);

        return servicesBusMessage;
    }

    public static async Task SendMessageAsync<TOptions>(this IAzureClientFactory<ServiceBusSender> clientFactory, ServiceBusMessage serviceBusMessage)
        where TOptions : IBusSenderOptions, new()
    {
        var serviceBusSender = clientFactory.CreateClient(TOptions.ClientName);
        await serviceBusSender.SendMessageAsync(serviceBusMessage);
    }
}
