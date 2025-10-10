#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Caching.Distributed;

public static class ReferenceTypeDistributedCacheWrapperExtensions
{
    public static async Task<T?> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken ct = default)
        where T : class
        => await distributedCache.GetReferenceTypeAsync<T>(key, ct);
}
