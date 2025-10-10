using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace MyNuget.Telemetry.Datadog;

public static class DatadogOptionsExtensions
{
    public static OptionsBuilder<OtlpExporterOptions> AddDatadogExporter(this IServiceCollection services, string optionsName)
        => services.AddOptions<OtlpExporterOptions>(optionsName)
            .Configure<IOptions<DatadogOptions>>((options, datadogOptions) =>
            {
                options.Headers = string.Format(datadogOptions.GetConfiguration(optionsName).HeaderFormat, datadogOptions.Value.ApiKey);
                options.Protocol = datadogOptions.GetConfiguration(optionsName).Protocol;
                options.Endpoint = datadogOptions.GetConfiguration(optionsName).Endpoint;
            });

    private static DatadogOtlpExporterConfiguration GetConfiguration(this IOptions<DatadogOptions> options, string optionsName)
        => options.Value.GetConfiguration(optionsName);

    private static DatadogOtlpExporterConfiguration GetConfiguration(this DatadogOptions options, string optionsName)
        => optionsName switch
        {
            DatadogOtlpExporterConfiguration.LogsOptionName => options.Logs,
            DatadogOtlpExporterConfiguration.MetricsOptionName => options.Metrics,
            DatadogOtlpExporterConfiguration.TracesOptionName => options.Traces,
            _ => throw new NotImplementedException()
        };
}
