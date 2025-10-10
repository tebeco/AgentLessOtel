using Azure.Messaging.ServiceBus;

namespace MyNuget.Azure.ServiceBus;

public static class ServiceBusMessageExtensions
{
    public const string SerializerPropertyName = "Olympe.Serializer";
    public const string JsonSerializerName = "System.Text.Json";
    public const string JsonBinarySerializerName = "BinaryData+System.Text.Json";
    public const string BinarySerializerName = "BinaryData";

    public static ServiceBusMessage WithBody<TMessage>(this ServiceBusMessage serviceBusMessage, TMessage messageDto, ServiceBusMessageSerializer serializer)
        where TMessage : notnull
        => serializer switch
        {
            ServiceBusMessageSerializer.BinaryData when messageDto is BinaryData binaryData => serviceBusMessage.WithBinaryBody(binaryData),
            ServiceBusMessageSerializer.SystemTextJson => serviceBusMessage.WithJsonBody(messageDto),
            ServiceBusMessageSerializer.JsonBinary => serviceBusMessage.WithJsonBinaryBody(messageDto),
            _ => throw new ArgumentException($"Unknown serializer: {serializer}")
        };

    public static ServiceBusMessage WithJsonBody<TMessage>(this ServiceBusMessage serviceBusMessage, TMessage message)
        where TMessage : notnull
        => serviceBusMessage.WithJsonBody(message, System.Text.Json.JsonSerializerOptions.Default);

    public static ServiceBusMessage WithJsonBody<TMessage>(this ServiceBusMessage serviceBusMessage, TMessage message, System.Text.Json.JsonSerializerOptions jsonSerializerOptions)
        where TMessage : notnull
    {
        serviceBusMessage.Body = new BinaryData(System.Text.Json.JsonSerializer.Serialize(message, jsonSerializerOptions));
        serviceBusMessage.ApplicationProperties[SerializerPropertyName] = ServiceBusMessageSerializer.SystemTextJson.ToSerializerName();

        return serviceBusMessage;
    }

    public static ServiceBusMessage WithJsonBinaryBody(this ServiceBusMessage serviceBusMessage, object jsonSerializable)
    {
        serviceBusMessage.Body = new BinaryData(jsonSerializable);
        serviceBusMessage.ApplicationProperties[SerializerPropertyName] = ServiceBusMessageSerializer.JsonBinary.ToSerializerName();

        return serviceBusMessage;
    }

    public static ServiceBusMessage WithBinaryBody(this ServiceBusMessage serviceBusMessage, BinaryData binaryData)
    {
        serviceBusMessage.Body = binaryData;
        serviceBusMessage.ApplicationProperties[SerializerPropertyName] = ServiceBusMessageSerializer.BinaryData.ToSerializerName();

        return serviceBusMessage;
    }

    public static string ToSerializerName(this ServiceBusMessageSerializer serializer)
        => serializer switch
        {
            ServiceBusMessageSerializer.SystemTextJson => JsonSerializerName,
            ServiceBusMessageSerializer.JsonBinary => JsonBinarySerializerName,
            ServiceBusMessageSerializer.BinaryData => BinarySerializerName,
            _ => throw new ArgumentException($"Unknown serializer: {serializer}")
        };
}
