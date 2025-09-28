using Microsoft.Extensions.DependencyInjection;
using MyNuget.Telemetry.Datadog;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DatadogServiceCollectionExtensions
{
    public static OpenTelemetryBuilder AddDatadog(this OpenTelemetryBuilder builder)
    {
        builder.Services
            .AddOptionsWithValidateOnStart<DatadogOptions>()
            .BindConfiguration(DatadogOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), """IF YOU SEE THIS ERROR YOU NEED TO RUN 'dotnet user-secrets --id agentless-otel-datadog set "Datadog:ApiKey" "<YOUR API KEY GOES HERE>"'"""); // 

        builder
            .AddDatadogOpenTelemetryLogs()
            .AddDatadogOpenTelemetryMetrics()
            .AddDatadogOpenTelemetryTraces();

        return builder;
    }

    public static OpenTelemetryBuilder AddDatadogOpenTelemetryLogs(this OpenTelemetryBuilder builder)
    {
        builder.Services.AddDatadogExporter(DatadogOtlpExporterConfiguration.LogsOptionName);

        builder.Services
            .AddOpenTelemetry()
            .WithLogging(
                loggerProviderBuilder =>
                {
                    loggerProviderBuilder.AddOtlpExporter(DatadogOtlpExporterConfiguration.LogsOptionName, configureExporter: null);
                },
                otelLoggerOptions =>
                {
                    otelLoggerOptions.IncludeFormattedMessage = true;
                    otelLoggerOptions.IncludeScopes = true;
                }
            );

        return builder;
    }

    public static OpenTelemetryBuilder AddDatadogOpenTelemetryMetrics(this OpenTelemetryBuilder builder)
    {
        builder.Services.AddDatadogExporter(DatadogOtlpExporterConfiguration.MetricsOptionName);

        builder.Services
            .AddOptions<MetricReaderOptions>(DatadogOtlpExporterConfiguration.MetricsOptionName)
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
                metrics.AddOtlpExporter(DatadogOtlpExporterConfiguration.MetricsOptionName, configure: null);
            });

        return builder;
    }

    public static OpenTelemetryBuilder AddDatadogOpenTelemetryTraces(this OpenTelemetryBuilder builder)
    {
        builder.Services.AddDatadogExporter(DatadogOtlpExporterConfiguration.TracesOptionName);

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddOtlpExporter(DatadogOtlpExporterConfiguration.TracesOptionName, configure: null);
            });

        return builder;
    }

}
