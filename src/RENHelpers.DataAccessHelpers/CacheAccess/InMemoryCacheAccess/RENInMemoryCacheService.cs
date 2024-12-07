using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RENHelpers.DataAccessHelpers.CacheHelpers;

namespace RENHelpers.DataAccessHelpers.CacheAccess.InMemoryCacheAccess;

public class RENInMemoryCacheService : IRENCacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> CacheKeys;
    private MemoryCacheEntryOptions _cacheOptions;
    private int _defaultAbsoluteExpirationHours;
    private int _defaultSlidingExpirationMinutes;

    public RENInMemoryCacheService(IConfiguration configuration, IMemoryCache cache)
    {
        SetDefaults(configuration);
        _cache = cache;
        CacheKeys = new HashSet<string>();
    }

    /// <summary>
    /// Retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <returns>The cached T value.</returns>
    public virtual T Get<T>(string cacheKey)
    {
        return (T)_cache.Get(cacheKey);
    }

    /// <summary>
    /// Asynchronously retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached T value.</returns>
    public virtual Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Get<T>(cacheKey);
        }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <returns>The cached string value.</returns>
    public virtual string Get(string cacheKey)
    {
        return _cache.Get(cacheKey).ToString();
    }

    /// <summary>
    /// Asynchronously retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached string value.</returns>
    public virtual Task<string> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Get(cacheKey);
        }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of data of type <typeparamref name="T"/> that matches the specified pattern.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached data items typed <typeparamref name="T"/>.</returns>
    public virtual List<T> GetByPattern<T>(string pattern = "*")
    {
        var keys = GetKeysFromPattern(pattern);
        return keys.Select(Get<T>).ToList();
    }

    /// <summary>
    /// Asynchronously retrieves a list of data of type <typeparamref name="T"/> that matches the specified pattern.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing a list of cached data items typed <typeparamref name="T"/>.</returns>
    public virtual Task<List<T>> GetByPatternAsync<T>(string pattern = "*", CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return GetByPattern<T>(pattern);
        }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of string keys that match the specified pattern in the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached string keys.</returns>
    public virtual List<string> GetByPattern(string pattern = "*")
    {
        var keys = GetKeysFromPattern(pattern);
        return keys.Select(Get).ToList();
    }

    /// <summary>
    /// Asynchronously retrieves a list of string keys that match the specified pattern in the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing a list of cached string keys.</returns>
    public virtual Task<List<string>> GetByPatternAsync(string pattern = "*", CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return GetByPattern(pattern);
        }, cancellationToken);
    }

    /// <summary>
    /// Sets data of type <typeparamref name="T"/> in the cache with optional expiration settings.
    /// </summary>
    /// <typeparam name="T">The type of data to store in the cache.</typeparam>
    /// <param name="cacheKey">The key for storing the data in the cache.</param>
    /// <param name="data">The data to cache of type <typeparamref name="T"/>.</param>
    /// <param name="absoluteExpiration">The optional absolute expiration time for the cached data.</param>
    /// <param name="slidingExpiration">The optional sliding expiration time for the cached data.</param>
    public virtual void Set<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var options = absoluteExpiration == null && slidingExpiration == null ? _cacheOptions : new MemoryCacheEntryOptions();

        if (absoluteExpiration != null)
            options.AbsoluteExpirationRelativeToNow = absoluteExpiration;
        if (slidingExpiration != null)
            options.SlidingExpiration = slidingExpiration;

        _cache.Set(cacheKey, data, options);

        CacheKeys.Add(cacheKey);
    }

    /// <summary>
    /// Asynchronously sets data of type <typeparamref name="T"/> in the cache with optional expiration settings.
    /// </summary>
    /// <typeparam name="T">The type of data to store in the cache.</typeparam>
    /// <param name="cacheKey">The key for storing the data in the cache.</param>
    /// <param name="data">The data to cache of type <typeparamref name="T"/>.</param>
    /// <param name="absoluteExpiration">The optional absolute expiration time for the cached data.</param>
    /// <param name="slidingExpiration">The optional sliding expiration time for the cached data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            Set(cacheKey, data, absoluteExpiration, slidingExpiration);
        }, cancellationToken);
    }

    /// <summary>
    /// Deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    public virtual void DeleteKeysByPattern(string pattern = "*")
    {
        var keysToDelete = GetKeysFromPattern(pattern);
        foreach (var key in keysToDelete) _cache.Remove(key);
    }

    /// <summary>
    /// Asynchronously deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DeleteKeysByPatternAsync(string pattern = "*", CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteKeysByPattern(pattern);
        }, cancellationToken);
    }

    /// <summary>
    /// Removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    public virtual void Remove(string cacheKey)
    {
        _cache.Remove(cacheKey);
        CacheKeys.Remove(cacheKey);
    }

    /// <summary>
    /// Asynchronously removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Remove(cacheKey);
        }, cancellationToken);
    }

    /// <summary>
    /// Clears the database.
    /// </summary>
    public virtual void Clear()
    {
        foreach (var key in CacheKeys) _cache.Remove(key);
    }

    /// <summary>
    /// Asynchronously clears the database.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task ClearAsync(CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Clear();
        }, cancellationToken);
    }

    private void ConsolidateKeys()
    {
        var invalidKeys = CacheKeys.Where(key => !_cache.TryGetValue(key, out _)).ToList();
        foreach (var key in invalidKeys) CacheKeys.Remove(key);
    }

    private void SetDefaults(IConfiguration configuration)
    {
        _defaultAbsoluteExpirationHours =
            int.Parse(configuration.GetSection("CacheConfiguration:InMemoryConfiguration:TimeConfiguration:AbsoluteExpirationInHours")?.Value ?? "12");
        _defaultSlidingExpirationMinutes =
            int.Parse(configuration.GetSection("CacheConfiguration:InMemoryConfiguration:TimeConfiguration:SlidingExpirationInMinutes")?.Value ?? "30");
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_defaultAbsoluteExpirationHours),
            SlidingExpiration = TimeSpan.FromSeconds(_defaultSlidingExpirationMinutes)
        };
    }

    private IEnumerable<string> GetKeysFromPattern(string pattern = "*")
    {
        ConsolidateKeys();

        return pattern == "*" ? CacheKeys : CacheKeys.Where(key => key.Contains(pattern));
    }
}