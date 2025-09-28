using Microsoft.Extensions.DependencyInjection;
using MyNuget.Telemetry.Datadog;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DatadogServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddDatadog(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptionsWithValidateOnStart<DatadogOptions>()
            .BindConfiguration(DatadogOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), """IF YOU SEE THIS ERROR YOU NEED TO RUN 'dotnet user-secrets --id agentless-otel-datadog set "Datadog:ApiKey" "<YOUR API KEY GOES HERE>"'"""); // 

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryLogs<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDatadogExporter(DatadogOptions.OtelLogsExporterOptionsName);

        builder.Services
            .AddOpenTelemetry()
            .WithLogging(
                loggerProviderBuilder =>
                {
                    loggerProviderBuilder.AddOtlpExporter(DatadogOptions.OtelLogsExporterOptionsName, configureExporter: null);
                },
                otelLoggerOptions =>
                {
                    otelLoggerOptions.IncludeFormattedMessage = true;
                    otelLoggerOptions.IncludeScopes = true;
                }
            );

        builder
            .AddDatadogOpenTelemetryLogs()
            .AddDatadogOpenTelemetryMetrics()
            .AddDatadogOpenTelemetryTraces();

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryMetrics<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDatadogExporter(DatadogOptions.OtelMetricsExporterOptionsName);

        builder.Services
            .AddOptions<MetricReaderOptions>(DatadogOptions.OtelMetricsExporterOptionsName)
            .Configure(options =>
            {
                // this one is event harder to find documentation about
                //https://docs.datadoghq.com/opentelemetry/guide/otlp_delta_temporality/?code-lang+=.+net&code-lang=.net&tab=.net&site=eu

                options.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                options.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                {
                    ExportIntervalMilliseconds = 5000,
                };
            });

        builder.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddOtlpExporter(DatadogOptions.OtelMetricsExporterOptionsName, configure: null);
            });

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryTraces<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDatadogExporter(DatadogOptions.OtelTracesExporterOptionsName);

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddOtlpExporter(DatadogOptions.OtelTracesExporterOptionsName, configure: null);
            });

        return builder;
    }

}
