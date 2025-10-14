using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyNuget.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class OpenTelemetryExtensions
{
    public static OpenTelemetryBuilder AddMyOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.EnableEnrichment();

        builder.Services.AddSingleton<MyResourceDetector>();

        return builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddDetector(sp => sp.GetRequiredService<MyResourceDetector>());
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });
    }
}
