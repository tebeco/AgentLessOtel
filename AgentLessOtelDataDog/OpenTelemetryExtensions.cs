using AgentLessOtelDataDog;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class OpenTelemetryExtensions
{
    public static TBuilder AddOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // currently Datadog OTEL endpoing are:
        // * not the same URL for logs/metrics/traces
        // * not the same header for logs/metrics/traces
        // so we need to configure 3 different exporters
        //
        // luckily we can use "named options pattern" for each specifics "DatadogOptions" of each 3 exporters

        builder
            .AddDatadogOpenTelemetryLogs()
            .AddDatadogOpenTelemetryMetrics()
            .AddDatadogOpenTelemetryTraces();

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryLogs<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        const string NamedOptions = "DatadogLogsExporter";

        // DOCS: it only covers OTEL env var, rest is undocumented
        // Dotnet docs is severly missing here
        // https://docs.datadoghq.com/opentelemetry/setup/agentless/logs/?site=eu

        builder.Services
            .AddOptions<OtlpExporterOptions>(NamedOptions)
            .Configure<IOptions<DatadogOptions>>((options, datadogOptions) =>
            {
                options.Headers = $"dd-api-key={datadogOptions.Value.ApiKey}";
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Endpoint = new Uri("https://http-intake.logs.datadoghq.eu/v1/logs");
            });

        builder.Services
            .AddOpenTelemetry()
            .WithLogging(
                loggerProviderBuilder =>
                {
                    loggerProviderBuilder.AddOtlpExporter(NamedOptions, otlpExportOptions => { });
                },
                otelLoggerOptions =>
                {
                    otelLoggerOptions.IncludeFormattedMessage = true;
                    otelLoggerOptions.IncludeScopes = true;
                }
            );

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryMetrics<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        const string NamedOptions = "DatadogMetricsExporter";

        // DOCS: it only covers OTEL env var, rest is undocumented
        // The docs is incorrect regarding "dd-otlp-source=", i was told by datadog support that it should be dd-otlp-source=datadog
        // Dotnet docs is severly missing here
        // https://docs.datadoghq.com/opentelemetry/setup/agentless/metrics/?site=eu

        builder.Services
            .AddOptions<OtlpExporterOptions>(NamedOptions)
            .Configure<IOptions<DatadogOptions>>((options, datadogOptions) =>
            {
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Endpoint = new Uri("https://api.datadoghq.eu/api/intake/otlp/v1/metrics");
                options.Headers = $"dd-api-key={datadogOptions.Value.ApiKey},dd-otlp-source=datadog";
            });

        // this one is event harder to find documentation about
        //https://docs.datadoghq.com/opentelemetry/guide/otlp_delta_temporality/?code-lang+=.+net&code-lang=.net&tab=.net&site=eu
        // 
        builder.Services
            .AddOptions<MetricReaderOptions>(NamedOptions)
            .Configure(options =>
            {
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
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(NamedOptions, (exporterOptions, metricReaderOptions) => { });
            });

        return builder;
    }

    public static TBuilder AddDatadogOpenTelemetryTraces<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        const string NamedOptions = "DatadogTracesExporter";

        // DOCS: it only covers OTEL env var, rest is undocumented
        // The docs is incorrect regarding "dd-otlp-source=", i was told by datadog support that it should be dd-otlp-source=datadog
        // Dotnet docs is severly missing here
        // https://docs.datadoghq.com/opentelemetry/setup/agentless/traces/?site=eu

        builder.Services
            .AddOptions<OtlpExporterOptions>(NamedOptions)
            .Configure<IOptions<DatadogOptions>>((options, datadogOptions) =>
            {
                options.Headers = $"dd-api-key={datadogOptions.Value.ApiKey},dd-otlp-source=datadog";
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Endpoint = new Uri("https://trace.agent.datadoghq.eu/v1/traces");
            });

        builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(builder.Environment.ApplicationName)
                    .AddSource(MyBackgroundService.MyBackgroundServiceActivityName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter(NamedOptions, exporterOptions => { });
            });

        return builder;
    }
}