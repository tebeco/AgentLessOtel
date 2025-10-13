namespace MyNuget.Telemetry;

public class InfrastructureOptions
{
    public const string SectionName = "Infrastructure";

    public string Team { get; set; } = "blabla";

    public string Environment { get; set; } = "local";

    public string Service { get; set; } = "nominal-api";

    public string Version { get; set; } = "1.2.3";
}
