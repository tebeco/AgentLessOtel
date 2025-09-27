using Microsoft.Extensions.DependencyInjection;

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

        builder.Services.AddHybridCache();
        return builder;
    }
}
