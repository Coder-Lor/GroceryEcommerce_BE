using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Interfaces.Services;

public class RedisCacheService(
    IDistributedCache cache,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<RedisCacheService> _logger = logger;
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data from cache with key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting data in cache with key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing data from cache with key: {Key}", key);
        }
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}