namespace MyNuget.Telemetry.Datadog;


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
}
