using GroceryEcommerce.Application.Interfaces.Services;

namespace GroceryEcommerce.Infrastructure.Services;

public class CacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetStringIfNotExistsAsync(string key, string value, TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetHashAsync<T>(string key, string field, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SetHashAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SetHashMultipleAsync<T>(string key, Dictionary<string, T> values, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveHashAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HashExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLeftPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> ListRightPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T?> ListLeftPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T?> ListRightPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double min = Double.NegativeInfinity,
        double max = Double.PositiveInfinity, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1,
        CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> SortedSetLengthAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExpireAtAsync(string key, DateTime expiry, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SetMultipleAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<long> RemoveMultipleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, object>> GetCacheInfoAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetMemoryUsageAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}