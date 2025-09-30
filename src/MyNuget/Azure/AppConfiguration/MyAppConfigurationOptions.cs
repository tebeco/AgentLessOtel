namespace MyNuget.Azure.AppConfiguration;

public class MyAppConfigurationOptions
{
    public const string SectionName = "MyAppConfiguration";

    public Uri? Endpoint { get; set; }

    public string? SharedPrefix { get; set; }

    public string? ApplicationPrefix { get; set; }

    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(10);
}
