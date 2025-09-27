namespace MyNuget.Telemetry;

public class InfrastructureOptions
{
    public const string SectionName = "Infrastructure";

    // ideally this should be understood by the datadog as what DD_ENV env var but properly as code
    public string Team { get; set; } = "blabla";

    // ideally this should be understood by the datadog as what DD_ENV env var but properly as code
    public string Environment { get; set; } = "local";

    // ideally this should be understood by the datadog as what DD_SERVICE env var but properly as code
    public string Service { get; set; } = "nominal-api";

    public string Version { get; set; } = "1.2.3";
}
