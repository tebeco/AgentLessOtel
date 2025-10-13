using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using StackExchange.Redis;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DistributedCacheServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMyCache(this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisCacheOptions = sp.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

                var multiplexer = ConnectionMultiplexer.Connect(redisCacheOptions.ConfigurationOptions!);
                redisCacheOptions.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(multiplexer);

                return multiplexer;
            });

        builder.Services.AddStackExchangeRedisCache(redisCacheOptions =>
        {
            redisCacheOptions.ConfigurationOptions = new();
            redisCacheOptions.ConfigurationOptions.EndPoints.Add("localhost:6379");
            redisCacheOptions.InstanceName = "MyWebApp:/";
        });

        builder.Services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache,
            };
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
            });

        return builder;
    }
}
