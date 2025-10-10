using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DistributedCacheServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMyCache(this IHostApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(builder =>
        {
            builder.Configuration = "localhost:6379";
            builder.InstanceName = "MyWebApp:/";
        });

        builder.Services.AddHybridCache(options =>
        {
        });

        builder
        .Services
        .AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing.AddRedisInstrumentation(options =>
            {
                options.SetVerboseDatabaseStatements = true;
            });
            tracing.ConfigureRedisInstrumentation(_ => { });
            tracing.AddInstrumentation(sp => sp.GetRequiredService<StackExchangeRedisInstrumentation>());
        });

        return builder;
    }
}
