#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text.Json;

namespace Microsoft.Extensions.Caching.Distributed;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetValueTypeAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken ct = default)
        where T : struct
    {
        try
        {
            var cachedString = await distributedCache.GetStringAsync(key, ct);

            return cachedString != null
                ? JsonSerializer.Deserialize<T>(cachedString)
                : null;
        }
        catch
        {
            return null;
        }
    }

    public static async Task<T?> GetReferenceTypeAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken ct = default)
        where T : class
    {
        try
        {
            var cachedString = await distributedCache.GetStringAsync(key, ct);

            return cachedString != null
                ? JsonSerializer.Deserialize<T>(cachedString)
                : null;
        }
        catch
        {
            return null;
        }
    }

    public static async Task RemoveAsync(this IDistributedCache distributedCache, string key, CancellationToken ct = default)
    {
        try
        {
            await distributedCache.RemoveAsync(key, ct);
        }
        catch { }
    }

    public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, TimeSpan duration, CancellationToken ct = default)
        where T : notnull
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(duration);

            await distributedCache.SetStringAsync(key, serializedValue, options, ct);
        }
        catch { }
    }
}
