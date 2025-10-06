using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Interfaces.Services;

public class RedisCacheService(
    IDistributedCache cache,
    IConnectionMultiplexer redis,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<RedisCacheService> _logger = logger;
    private readonly IConnectionMultiplexer _redis = redis;
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
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

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
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

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing data from cache with key: {Key}", key);
            return false;
        }
    }

    public async Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pattern));

        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());
        var keys = server.Keys(pattern: $"*{pattern}*", pageSize: 1000);

        long count = 0;
        foreach (var key in keys)
        {
            if (await _db.KeyDeleteAsync(key))
                count++;
        }
        return count;
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
            return cached;

        var value = factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        return await _db.KeyExpireAsync(key, expiration);
    }

    public async Task<bool> ExpireAtAsync(string key, DateTime expiration, CancellationToken cancellationToken = default)
    {
        return await _db.KeyExpireAsync(key, expiration);
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyTimeToLiveAsync(key);
    }

    public async Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.KeyPersistAsync(key);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _cache.GetStringAsync(key, cancellationToken);
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };
        await _cache.SetStringAsync(key, value, options, cancellationToken);
    }

    public async Task<bool> SetStringIfNotExistsAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        return await _db.StringSetAsync(key, value, expiration, When.NotExists);
    }

    public async Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        var result = new Dictionary<string, T?>();
        foreach (var key in keys)
        {
            result[key] = await GetAsync<T>(key, cancellationToken);
        }
        return result;
    }

    public async Task SetMultipleAsync<T>(Dictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        foreach (var item in items)
        {
            await SetAsync(item.Key, item.Value, expiration, cancellationToken);
        }
    }

    public async Task<long> RemoveMultipleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        return await _db.KeyDeleteAsync(redisKeys);
    }

    public async Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        await RemoveByPatternAsync(pattern, cancellationToken);
    }

    public async Task<T?> GetHashAsync<T>(string key, string field, CancellationToken cancellationToken = default) where T : class
    {
        var value = await _db.HashGetAsync(key, field);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task SetHashAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        await _db.HashSetAsync(key, field, serialized);
    }

    public async Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var entries = await _db.HashGetAllAsync(key);
        return entries.ToDictionary(
            e => e.Name.ToString(),
            e => JsonSerializer.Deserialize<T>(e.Value!)
        );
    }

    public async Task SetHashMultipleAsync<T>(string key, Dictionary<string, T> values, CancellationToken cancellationToken = default) where T : class
    {
        var entries = values.Select(v => new HashEntry(v.Key, JsonSerializer.Serialize(v.Value))).ToArray();
        await _db.HashSetAsync(key, entries);
    }

    public async Task<bool> HashExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        return await _db.HashExistsAsync(key, field);
    }

    public async Task<bool> RemoveHashAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        return await _db.HashDeleteAsync(key, field);
    }

    public async Task<long> ListLeftPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.ListLeftPushAsync(key, serialized);
    }

    public async Task<long> ListRightPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.ListRightPushAsync(key, serialized);
    }

    public async Task<T?> ListLeftPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var value = await _db.ListLeftPopAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task<T?> ListRightPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var value = await _db.ListRightPopAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.ListLengthAsync(key);
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start, long stop, CancellationToken cancellationToken = default) where T : class
    {
        var values = await _db.ListRangeAsync(key, start, stop);
        return values.Select(v => JsonSerializer.Deserialize<T>(v!)!).ToList();
    }

    public async Task<long> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.SetAddAsync(key, serialized) ? 1 : 0;
    }

    public async Task<long> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.SetRemoveAsync(key, serialized) ? 1 : 0;
    }

    public async Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.SetContainsAsync(key, serialized);
    }

    public async Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.SetLengthAsync(key);
    }

    public async Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var values = await _db.SetMembersAsync(key);
        return values.Select(v => JsonSerializer.Deserialize<T>(v!)!).ToList();
    }

    public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        return await _db.SortedSetAddAsync(key, serialized, score);
    }

    public async Task<long> SortedSetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _db.SortedSetLengthAsync(key);
    }

    public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start, long stop, CancellationToken cancellationToken = default) where T : class
    {
        var values = await _db.SortedSetRangeByRankAsync(key, start, stop);
        return values.Select(v => JsonSerializer.Deserialize<T>(v!)!).ToList();
    }

    public async Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double start, double stop, CancellationToken cancellationToken = default) where T : class
    {
        var values = await _db.SortedSetRangeByScoreAsync(key, start, stop);
        return values.Select(v => JsonSerializer.Deserialize<T>(v!)!).ToList();
    }

    public async Task<Dictionary<string, object>> GetCacheInfoAsync(CancellationToken cancellationToken = default)
    {
        var info = new Dictionary<string, object>();
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var serverInfo = await server.InfoAsync();

        foreach (var section in serverInfo)
        {
            foreach (var item in section)
            {
                info[item.Key] = item.Value;
            }
        }

        return info;
    }

    public async Task<long> GetMemoryUsageAsync(CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var info = await server.InfoAsync("memory");
        var memorySection = info.FirstOrDefault(s => s.Key == "memory");

        if (memorySection.Any())
        {
            var usedMemory = memorySection.FirstOrDefault(i => i.Key == "used_memory");
            if (long.TryParse(usedMemory.Value, out var memory))
                return memory;
        }

        return 0;
    }
}
