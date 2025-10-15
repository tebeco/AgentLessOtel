using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace MyWebApp;

[Route("/azf")]
[ApiController]
public class AzfController(
    IAzureClientFactory<ServiceBusSender> serviceBusClientFactory,
    IAzureClientFactory<EventHubProducerClient> eventHubProducerClientFactory
    ) : ControllerBase
{
    [HttpPost("service-bus")]
    public async Task<NoContent> ServiceBusTrigger()
    {
        var serviceBusSender = serviceBusClientFactory.CreateClient("MyServiceBusTopicSender");

        await serviceBusSender.SendMessageAsync(new ServiceBusMessage("Hello, Service Bus!"));

        return TypedResults.NoContent();
    }

    [HttpPost("event-hub")]
    public async Task<NoContent> EventHubTrigger()
    {
        await using var producer = eventHubProducerClientFactory.CreateClient("MyEventHubProducerClient");
        using var eventBatch = await producer.CreateBatchAsync();

        eventBatch.TryAdd(new EventData("First"));
        eventBatch.TryAdd(new EventData("Second"));

        await producer.SendAsync(eventBatch);

        return TypedResults.NoContent();
    }

    [HttpPost("storage-queue")]
    public async Task<NoContent> StorageQueueTrigger()
    {
        await Task.Yield();
        return TypedResults.NoContent();
    }
}
