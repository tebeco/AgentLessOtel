using OpenTelemetry.Exporter;

namespace MyNuget.Telemetry.Datadog;

public class DatadogOtlpExporterConfiguration
{
    public const string LogsOptionName = "DatadogOtelLogsExporter";
    public const string MetricsOptionName = "DatadogOtelMetricsExporter";
    public const string TracesOptionName = "DatadogOtelTracesExporter";

    public required string OptionsName { get; set; }

    public required Uri Endpoint { get; set; }

    public required string HeaderFormat { get; set; }

    public required OtlpExportProtocol Protocol { get; set; }
}
