using System.Diagnostics;
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
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // used in startup because specific ActivitySource needs to be specifically "Added" into the .WithTracing
    public const string MyActivitySourceName = "MyNuget.ActivitySource";
    public static readonly ActivitySource ActivitySource = new(OpenTelemetryExtensions.MyActivitySourceName);
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                    .AddSource(MyActivitySourceName)
                    .AddSource("MyWebApp.ActivitySource") // TODO: find a way to add this from consumer side
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });
    }
}
