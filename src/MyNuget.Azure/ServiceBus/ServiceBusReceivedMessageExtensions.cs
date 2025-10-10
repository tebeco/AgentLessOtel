using Azure.Messaging.ServiceBus;

namespace MyNuget.Azure.ServiceBus;

public static class ServiceBusReceivedMessageExtensions
{
    public static T Deserialize<T>(this ServiceBusReceivedMessage message) where T : class
    {
        var serviceBusMessageSerializer = ToServiceBusMessageSerializer(message.ApplicationProperties.GetValueOrDefault(ServiceBusMessageExtensions.SerializerPropertyName)?.ToString());
        return (serviceBusMessageSerializer switch
        {
            ServiceBusMessageSerializer.SystemTextJson => System.Text.Json.JsonSerializer.Deserialize<T>(message.Body.ToString())!,
            ServiceBusMessageSerializer.JsonBinary => message.Body.ToObjectFromJson<T>(),
            _ => throw new ArgumentException($"Unknown serviceBusMessageSerializer: {serviceBusMessageSerializer}")
        })!;
    }

    public static ServiceBusMessageSerializer ToServiceBusMessageSerializer(this string? serializerName)
        => serializerName switch
        {
            ServiceBusMessageExtensions.JsonSerializerName => ServiceBusMessageSerializer.SystemTextJson,
            ServiceBusMessageExtensions.JsonBinarySerializerName => ServiceBusMessageSerializer.JsonBinary,
            ServiceBusMessageExtensions.BinarySerializerName => ServiceBusMessageSerializer.BinaryData,
            _ => ServiceBusMessageSerializer.SystemTextJson
        };
}
