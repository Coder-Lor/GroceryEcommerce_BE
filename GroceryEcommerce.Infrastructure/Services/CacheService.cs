using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace GroceryEcommerce.Infrastructure.Services;

public class CacheService(IMemoryCache cache) : ICacheService
{
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);
        
        cache.Set(key, value, options);
        _keys.TryAdd(key, 0);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cache.Remove(key);
        _keys.TryRemove(key, out _);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(cache.TryGetValue(key, out _));
    }

    public Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var regex = new Regex(pattern.Replace("*", ".*"));
        var keysToRemove = _keys.Keys.Where(k => regex.IsMatch(k)).ToList();
        
        foreach (var key in keysToRemove)
        {
            cache.Remove(key);
            _keys.TryRemove(key, out _);
        }
        
        return Task.FromResult((long)keysToRemove.Count);
    }

    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        cache.TryGetValue(key, out string? value);
        return Task.FromResult(value);
    }

    public Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);
        
        cache.Set(key, value, options);
        _keys.TryAdd(key, 0);
        return Task.CompletedTask;
    }

    public Task<bool> SetStringIfNotExistsAsync(string key, string value, TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(key, out _))
            return Task.FromResult(false);
        
        var options = new MemoryCacheEntryOptions();
        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);
        
        cache.Set(key, value, options);
        _keys.TryAdd(key, 0);
        return Task.FromResult(true);
    }

    public Task<T?> GetHashAsync<T>(string key, string field, CancellationToken cancellationToken = default) where T : class
    {
        cache.TryGetValue(key, out Dictionary<string, T>? hash);
        return Task.FromResult(hash?.GetValueOrDefault(field));
    }

    public Task SetHashAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out Dictionary<string, T>? hash))
        {
            hash = new Dictionary<string, T>();
            cache.Set(key, hash);
            _keys.TryAdd(key, 0);
        }
        
        hash![field] = value;
        return Task.CompletedTask;
    }

    public Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        cache.TryGetValue(key, out Dictionary<string, T>? hash);
        return Task.FromResult(hash?.ToDictionary(kvp => kvp.Key, kvp => (T?)kvp.Value) ?? new Dictionary<string, T?>());
    }

    public Task SetHashMultipleAsync<T>(string key, Dictionary<string, T> values, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out Dictionary<string, T>? hash))
        {
            hash = new Dictionary<string, T>();
            cache.Set(key, hash);
            _keys.TryAdd(key, 0);
        }
        
        foreach (var kvp in values)
            hash![kvp.Key] = kvp.Value;
        
        return Task.CompletedTask;
    }

    public Task<bool> RemoveHashAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out Dictionary<string, object>? hash))
            return Task.FromResult(false);
        
        return Task.FromResult(hash.Remove(field));
    }

    public Task<bool> HashExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out Dictionary<string, object>? hash))
            return Task.FromResult(false);
        
        return Task.FromResult(hash.ContainsKey(field));
    }

    public Task<long> ListLeftPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out List<T>? list))
        {
            list = new List<T>();
            cache.Set(key, list);
            _keys.TryAdd(key, 0);
        }
        
        list!.Insert(0, value);
        return Task.FromResult((long)list.Count);
    }

    public Task<long> ListRightPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out List<T>? list))
        {
            list = new List<T>();
            cache.Set(key, list);
            _keys.TryAdd(key, 0);
        }
        
        list!.Add(value);
        return Task.FromResult((long)list.Count);
    }

    public Task<T?> ListLeftPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out List<T>? list) || list.Count == 0)
            return Task.FromResult<T?>(null);
        
        var value = list[0];
        list.RemoveAt(0);
        return Task.FromResult<T?>(value);
    }

    public Task<T?> ListRightPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out List<T>? list) || list.Count == 0)
            return Task.FromResult<T?>(null);
        
        var value = list[^1];
        list.RemoveAt(list.Count - 1);
        return Task.FromResult<T?>(value);
    }

    public Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out List<T>? list))
            return Task.FromResult(new List<T>());
        
        var endIndex = stop == -1 ? list.Count : (int)stop + 1;
        return Task.FromResult(list.Skip((int)start).Take(endIndex - (int)start).ToList());
    }

    public Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out List<object>? list))
            return Task.FromResult(0L);
        
        return Task.FromResult((long)list.Count);
    }

    public Task<long> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out HashSet<T>? set))
        {
            set = new HashSet<T>();
            cache.Set(key, set);
            _keys.TryAdd(key, 0);
        }
        
        return Task.FromResult(set!.Add(value) ? 1L : 0L);
    }

    public Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out HashSet<T>? set))
            return Task.FromResult(false);
        
        return Task.FromResult(set.Contains(value));
    }

    public Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out HashSet<T>? set))
            return Task.FromResult(new List<T>());
        
        return Task.FromResult(set.ToList());
    }

    public Task<long> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out HashSet<T>? set))
            return Task.FromResult(0L);
        
        return Task.FromResult(set.Remove(value) ? 1L : 0L);
    }

    public Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out HashSet<object>? set))
            return Task.FromResult(0L);
        
        return Task.FromResult((long)set.Count);
    }

    public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out SortedDictionary<double, T>? sortedSet))
        {
            sortedSet = new SortedDictionary<double, T>();
            cache.Set(key, sortedSet);
            _keys.TryAdd(key, 0);
        }
        
        sortedSet![score] = value;
        return Task.FromResult(true);
    }

    public Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double min = Double.NegativeInfinity,
        double max = Double.PositiveInfinity, CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out SortedDictionary<double, T>? sortedSet))
            return Task.FromResult(new List<T>());
        
        return Task.FromResult(sortedSet.Where(kvp => kvp.Key >= min && kvp.Key <= max).Select(kvp => kvp.Value).ToList());
    }

    public Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1,
        CancellationToken cancellationToken = default) where T : class
    {
        if (!cache.TryGetValue(key, out SortedDictionary<double, T>? sortedSet))
            return Task.FromResult(new List<T>());
        
        var endIndex = stop == -1 ? sortedSet.Count : (int)stop + 1;
        return Task.FromResult(sortedSet.Values.Skip((int)start).Take(endIndex - (int)start).ToList());
    }

    public Task<long> SortedSetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out SortedDictionary<double, object>? sortedSet))
            return Task.FromResult(0L);
        
        return Task.FromResult((long)sortedSet.Count);
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out object? value))
            return Task.FromResult(false);
        
        cache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiry));
        return Task.FromResult(true);
    }

    public Task<bool> ExpireAtAsync(string key, DateTime expiry, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out object? value))
            return Task.FromResult(false);
        
        cache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiry));
        return Task.FromResult(true);
    }

    public Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TimeSpan?>(null);
    }

    public Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!cache.TryGetValue(key, out object? value))
            return Task.FromResult(false);
        
        cache.Set(key, value);
        return Task.FromResult(true);
    }

    public Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        var result = new Dictionary<string, T?>();
        foreach (var key in keys)
        {
            cache.TryGetValue(key, out T? value);
            result[key] = value;
        }
        return Task.FromResult(result);
    }

    public async Task SetMultipleAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        foreach (var kvp in values)
            await SetAsync(kvp.Key, kvp.Value, expiry, cancellationToken);
    }

    public Task<long> RemoveMultipleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        long count = 0;
        foreach (var key in keys)
        {
            cache.Remove(key);
            _keys.TryRemove(key, out _);
            count++;
        }
        return Task.FromResult(count);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null) return cached;

        var value = await factory();
        await SetAsync(key, value, expiry, cancellationToken);
        return value;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null) return cached;

        var value = factory();
        await SetAsync(key, value, expiry, cancellationToken);
        return value;
    }

    public async Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        await RemoveByPatternAsync(pattern, cancellationToken);
    }

    public Task<Dictionary<string, object>> GetCacheInfoAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<string, object>
        {
            { "TotalKeys", _keys.Count },
            { "CacheType", "MemoryCache" }
        });
    }

    public Task<long> GetMemoryUsageAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0L);
    }
}
