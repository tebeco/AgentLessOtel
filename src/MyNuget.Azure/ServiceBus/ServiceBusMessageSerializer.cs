namespace MyNuget.Azure.ServiceBus;

public enum ServiceBusMessageSerializer
{
    SystemTextJson = 1, // System.Text.Json
    JsonBinary = 2, // BinaryData + System.Text.Json
    BinaryData = 3 // BinaryData
}

