namespace Microsoft.Extensions.Hosting;


////////////////////////////////////////////////////////////////////////////////////////////////////////////
// DATADOG SPECIFIC INTEGRATION
// goal being not to rely on magic value beside telling datadog what to map specifically
// sounds like this should be part of the "OtlpExporterOptions" 
////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class DatadogOptions
{
    public const string SectionName = "Datadog";

    // This represents the equivalent of "DD_API_KEY" env var but properly as code
    public required string ApiKey { get; set; }

    // ideally this should be understood by the datadog as what DD_ENV env var but properly as code
    public string Team { get; set; } = "blabla";

    // ideally this should be understood by the datadog as what DD_ENV env var but properly as code
    public string Environment { get; set; } = "local";

    // ideally this should be understood by the datadog as what DD_SERVICE env var but properly as code
    public string Service { get; set; } = "nominal-api";

    public string Version { get; set; } = "1.2.3";
}
