using OpenTelemetry.Exporter;

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

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// Logs
    /// https://docs.datadoghq.com/opentelemetry/setup/agentless/logs/?site=eu
    public DatadogOtlpExporterConfiguration Logs { get; set; } = new DatadogOtlpExporterConfiguration
    {
        OptionsName = DatadogOtlpExporterConfiguration.LogsOptionName,
        Endpoint = new("https://http-intake.logs.datadoghq.eu/v1/logs"),
        HeaderFormat = "dd-api-key={0}",
        Protocol = OtlpExportProtocol.HttpProtobuf
    };

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// Metrics
    /// https://docs.datadoghq.com/opentelemetry/setup/agentless/metrics/?site=eu
    public DatadogOtlpExporterConfiguration Metrics { get; set; } = new DatadogOtlpExporterConfiguration
    {
        OptionsName = DatadogOtlpExporterConfiguration.MetricsOptionName,
        Endpoint = new("https://api.datadoghq.eu/api/intake/otlp/v1/metrics"),
        HeaderFormat = "dd-api-key={0},dd-otel-source=datadog",
        Protocol = OtlpExportProtocol.HttpProtobuf
    };

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// Traces
    /// https://docs.datadoghq.com/opentelemetry/setup/agentless/traces/?site=eu
    public DatadogOtlpExporterConfiguration Traces { get; set; } = new DatadogOtlpExporterConfiguration
    {
        OptionsName = DatadogOtlpExporterConfiguration.TracesOptionName,
        Endpoint = new("https://trace.agent.datadoghq.eu/v1/traces"),
        HeaderFormat = "dd-api-key={0},dd-otel-source=datadog",
        Protocol = OtlpExportProtocol.HttpProtobuf
    };
}
