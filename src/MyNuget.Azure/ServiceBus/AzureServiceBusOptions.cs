namespace MyNuget.Azure.ServiceBus;

public class AzureServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public required string ConnectionString { get; set; }
}
