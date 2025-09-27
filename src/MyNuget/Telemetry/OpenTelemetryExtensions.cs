using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyNuget.Telemetry.Datadog;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class OpenTelemetryExtensions
{
    public static IHostApplicationBuilder AddOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.EnableEnrichment();

        // currently Datadog OTEL endpoing are:
        // * not the same URL for logs/metrics/traces
        // * not the same header for logs/metrics/traces
        // so we need to configure 3 different exporters
        //
        // luckily we can use "named options pattern" for each specifics "DatadogOptions" of each 3 exporters

        // Resource detector is used to add custom attributes to all logs/metrics/traces
        // https://opentelemetry.io/docs/specs/semconv/resource/#semantic-attributes-with-dedicated-environment-variable
        // https://opentelemetry.io/docs/languages/dotnet/resources/
        builder.Services.AddSingleton<DatadogResourceDetector>();

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
                    loggerProviderBuilder.ConfigureResource(resource =>
                    {
                        resource.AddDetector(sp => sp.GetRequiredService<DatadogResourceDetector>());
                    });
                    loggerProviderBuilder.AddOtlpExporter(NamedOptions, configureExporter: null);
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
                metrics.ConfigureResource(resource =>
                {
                    resource.AddDetector(sp => sp.GetRequiredService<DatadogResourceDetector>());
                });

                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(NamedOptions, configure: null);
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
                tracing.ConfigureResource(resource =>
                {
                    resource.AddDetector(sp => sp.GetRequiredService<DatadogResourceDetector>());
                });

                tracing
                    .AddSource(builder.Environment.ApplicationName)
                    .AddSource("MyWebApp.ActivitySource") // TODO: find a way to add this from consumer side
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter(NamedOptions, configure: null);
            });

        return builder;
    }
}