namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ICacheService
{
    // Basic cache operations
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<long> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    // String operations
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<bool> SetStringIfNotExistsAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    // Hash operations
    Task<T?> GetHashAsync<T>(string key, string field, CancellationToken cancellationToken = default) where T : class;
    Task SetHashAsync<T>(string key, string field, T value, CancellationToken cancellationToken = default) where T : class;
    Task<Dictionary<string, T?>> GetHashAllAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetHashMultipleAsync<T>(string key, Dictionary<string, T> values, CancellationToken cancellationToken = default) where T : class;
    Task<bool> RemoveHashAsync(string key, string field, CancellationToken cancellationToken = default);
    Task<bool> HashExistsAsync(string key, string field, CancellationToken cancellationToken = default);

    // List operations
    Task<long> ListLeftPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    Task<long> ListRightPushAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    Task<T?> ListLeftPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<T?> ListRightPopAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class;
    Task<long> ListLengthAsync(string key, CancellationToken cancellationToken = default);

    // Set operations
    Task<long> SetAddAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    Task<bool> SetContainsAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    Task<List<T>> SetMembersAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<long> SetRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    Task<long> SetLengthAsync(string key, CancellationToken cancellationToken = default);

    // Sorted Set operations
    Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default) where T : class;
    Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, CancellationToken cancellationToken = default) where T : class;
    Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, CancellationToken cancellationToken = default) where T : class;
    Task<long> SortedSetLengthAsync(string key, CancellationToken cancellationToken = default);

    // Expiry operations
    Task<bool> ExpireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<bool> ExpireAtAsync(string key, DateTime expiry, CancellationToken cancellationToken = default);
    Task<TimeSpan?> GetTimeToLiveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> PersistAsync(string key, CancellationToken cancellationToken = default);

    // Batch operations
    Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class;
    Task SetMultipleAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task<long> RemoveMultipleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    // Cache patterns
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task InvalidateByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    // Cache statistics
    Task<Dictionary<string, object>> GetCacheInfoAsync(CancellationToken cancellationToken = default);
    Task<long> GetMemoryUsageAsync(CancellationToken cancellationToken = default);
}