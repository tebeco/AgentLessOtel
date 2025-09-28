using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public const string OtelLogsExporterOptionsName = "DatadogOtelLogsExporter";
    public Uri LogsIngestionEndpoint { get; set; } = new("https://http-intake.logs.datadoghq.eu/v1/logs");
    ////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// Metrics
    /// https://docs.datadoghq.com/opentelemetry/setup/agentless/metrics/?site=eu
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public const string OtelMetricsExporterOptionsName = "DatadogOtelMetricsExporter";
    public Uri MetricsIngestionEndpoint { get; set; } = new("https://api.datadoghq.eu/api/intake/otlp/v1/metrics");

    ////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// Traces
    /// https://docs.datadoghq.com/opentelemetry/setup/agentless/traces/?site=eu
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public const string OtelTracesExporterOptionsName = "DatadogOtelTracesExporter";
    public Uri TracesIngestionEndpoint { get; set; } = new("https://trace.agent.datadoghq.eu/v1/traces");
}

public static class DatadogOptionsExtensions
{
    public static OtlpExporterOptions Bind(this OtlpExporterOptions options, IOptions<DatadogOptions> datadogOptions)
    {
        options.Headers = $"dd-api-key={datadogOptions.Value.ApiKey},dd-otlp-source=datadog";
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
        options.Endpoint = datadogOptions.Value.TracesIngestionEndpoint;

        return options;
    }

    public static OptionsBuilder<OtlpExporterOptions> AddDatadogExporter(this IServiceCollection services, string optionsName)
        => services.AddOptions<OtlpExporterOptions>(optionsName)
            .Configure<IOptions<DatadogOptions>>((options, datadogOptions) =>
            {
                options.Bind(datadogOptions);
            });
}
