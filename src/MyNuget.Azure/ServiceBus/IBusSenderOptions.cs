namespace MyNuget.Azure.ServiceBus;

public interface IBusSenderOptions
{
    static abstract string SectionName { get; set; }

    static abstract string ClientName { get; set; }

    string QueueOrTopicName { get; set; }
}
