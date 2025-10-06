using System.Text.Json;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    // Basic cache operations
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(cachedValue))
                return null;

            return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions();

            if (expiry.HasValue)
                options.SetAbsoluteExpiration(expiry.Value);

            await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetStringAsync(key, cancellationToken);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Note: This is a simplified implementation. In a real Redis implementation,
        // you would need to use SCAN with MATCH pattern to find keys and then delete them.
        // For now, we'll return 0 as this requires more complex Redis operations.
        _logger.LogWarning("RemoveByPattern is not fully implemented for pattern: {Pattern}", pattern);
        return 0;
    }

    // String operations
    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetStringAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting string cache value for key: {Key}", key);
            return null;
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions();
            if (expiry.HasValue)
                options.SetAbsoluteExpiration(expiry.Value);

            await _cache.SetStringAsync(key, value, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting string cache value for key: {Key}", key);
        }
    }

    public async Task<bool> SetStringIfNotExistsAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(existing))
                return false;

            await SetStringAsync(key, value, expiry, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting string cache value if not exists for key: {Key}", key);
            return false;
        }
    }

    // Hash operations (simplified implementation)
    public async Task<T?> GetHashAsync<T>(string key, string field, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var hashKey = $"{key}:{field}";
            return await GetAsync<T>(hashKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash cache value for key: {Key}, field: {Field}", key, field);
            return null;
        }
    }

    public async Task SetHashAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var hashKey = $"{key}:{field}";
            await SetAsync(hashKey, value, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash cache value for key: {Key}, field: {Field}", key, field);
        }
    }

    public async Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        // Simplified implementation - in real Redis, you would use HGETALL
        _logger.LogWarning("GetHashAllAsync is not fully implemented for key: {Key}", key);
        return new Dictionary<string, T?>();
    }

    public async Task SetHashMultipleAsync<T>(string key, Dictionary<string, T> values, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            foreach (var kvp in values)
            {
                await SetHashAsync(key, kvp.Key, kvp.Value, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple hash cache values for key: {Key}", key);
        }
    }

    public async Task<bool> RemoveHashAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var hashKey = $"{key}:{field}";
            return await RemoveAsync(hashKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing hash cache value for key: {Key}, field: {Field}", key, field);
            return false;
        }
    }

    public async Task<bool> HashExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        try
        {
            var hashKey = $"{key}:{field}";
            return await ExistsAsync(hashKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking hash cache existence for key: {Key}, field: {Field}", key, field);
            return false;
        }
    }

    // List operations (simplified implementation)
    public async Task<long> ListLeftPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("ListLeftPushAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    public async Task<long> ListRightPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("ListRightPushAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    public async Task<T?> ListLeftPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("ListLeftPopAsync is not fully implemented for key: {Key}", key);
        return null;
    }

    public async Task<T?> ListRightPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("ListRightPopAsync is not fully implemented for key: {Key}", key);
        return null;
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("ListRangeAsync is not fully implemented for key: {Key}", key);
        return new List<T>();
    }

    public async Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("ListLengthAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    // Set operations (simplified implementation)
    public async Task<long> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SetAddAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    public async Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SetContainsAsync is not fully implemented for key: {Key}", key);
        return false;
    }

    public async Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SetMembersAsync is not fully implemented for key: {Key}", key);
        return new List<T>();
    }

    public async Task<long> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SetRemoveAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    public async Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("SetLengthAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    // Sorted Set operations (simplified implementation)
    public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SortedSetAddAsync is not fully implemented for key: {Key}", key);
        return false;
    }

    public async Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SortedSetRangeByScoreAsync is not fully implemented for key: {Key}", key);
        return new List<T>();
    }

    public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogWarning("SortedSetRangeByRankAsync is not fully implemented for key: {Key}", key);
        return new List<T>();
    }

    public async Task<long> SortedSetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("SortedSetLengthAsync is not fully implemented for key: {Key}", key);
        return 0;
    }

    // Expiry operations
    public async Task<bool> ExpireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingValue = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(existingValue))
                return false;

            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(expiry);
            await _cache.SetStringAsync(key, existingValue, options, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiry for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExpireAtAsync(string key, DateTime expiry, CancellationToken cancellationToken = default)
    {
        try
        {
            var timeSpan = expiry - DateTime.UtcNow;
            if (timeSpan <= TimeSpan.Zero)
                return false;

            return await ExpireAsync(key, timeSpan, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiry at for key: {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        // This is not directly supported by IDistributedCache
        _logger.LogWarning("GetTimeToLiveAsync is not fully implemented for key: {Key}", key);
        return null;
    }

    public async Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default)
    {
        // This is not directly supported by IDistributedCache
        _logger.LogWarning("PersistAsync is not fully implemented for key: {Key}", key);
        return false;
    }

    // Batch operations
    public async Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        var result = new Dictionary<string, T?>();
        foreach (var key in keys)
        {
            var value = await GetAsync<T>(key, cancellationToken);
            result[key] = value;
        }
        return result;
    }

    public async Task SetMultipleAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        foreach (var kvp in values)
        {
            await SetAsync(kvp.Key, kvp.Value, expiry, cancellationToken);
        }
    }

    public async Task<long> RemoveMultipleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        long removedCount = 0;
        foreach (var key in keys)
        {
            if (await RemoveAsync(key, cancellationToken))
                removedCount++;
        }
        return removedCount;
    }

    // Cache patterns
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
            return cachedValue;

        var value = await factory();
        await SetAsync(key, value, expiry, cancellationToken);
        return value;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
            return cachedValue;

        var value = factory();
        await SetAsync(key, value, expiry, cancellationToken);
        return value;
    }

    public async Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // This would require more complex Redis operations
        _logger.LogWarning("InvalidateByPatternAsync is not fully implemented for pattern: {Pattern}", pattern);
    }

    // Cache statistics
    public async Task<Dictionary<string, object>> GetCacheInfoAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("GetCacheInfoAsync is not fully implemented");
        return new Dictionary<string, object>();
    }

    public async Task<long> GetMemoryUsageAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("GetMemoryUsageAsync is not fully implemented");
        return 0;
    }
}
