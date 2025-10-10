using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyNuget.Telemetry.Lgtm;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class LgtmServiceCollectionExtensions
{
    public static OpenTelemetryBuilder AddLgtm(this OpenTelemetryBuilder builder)
    {
        builder.Services
            .AddOptionsWithValidateOnStart<LgtmOptions>()
            .BindConfiguration(LgtmOptions.SectionName);

        builder.Services
            .AddOptions<OtlpExporterOptions>()
            .Configure<IOptions<LgtmOptions>>((options, lgtmOptions) =>
            {
                options.Endpoint = lgtmOptions.Value.Endpoint;
                options.Protocol = lgtmOptions.Value.Protocol;
            });

        builder.Services
            .AddOpenTelemetry()
            .WithLogging(logging => logging.AddOtlpExporter())
            .WithMetrics(metrics =>
            {
                metrics.AddOtlpExporter((options, metricReaderOptions) =>
                {
                    metricReaderOptions.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                    {
                        ExportIntervalMilliseconds = 5000,
                    };
                });
            })
            .WithTracing(tracing => tracing.AddOtlpExporter())
            ;

        return builder;
    }
}
