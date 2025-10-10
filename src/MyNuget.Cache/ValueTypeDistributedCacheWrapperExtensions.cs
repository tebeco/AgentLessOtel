#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Caching.Distributed;

public static class ValueTypeDistributedCacheWrapperExtensions
{
    public static async Task<T?> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken ct = default)
        where T : struct
        => await distributedCache.GetValueTypeAsync<T>(key, ct);
}
