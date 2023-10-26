using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace RENHelpers.DataAccessHelpers.CacheHelpers;

public class RENInMemoryCacheService : IRENCacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> CacheKeys;
    private MemoryCacheEntryOptions _cacheOptions;
    private int _defaultAbsoluteExpirationHours;
    private int _defaultSlidingExpirationMinutes;

    public RENInMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
        CacheKeys = new HashSet<string>();
    }

    public virtual T Get<T>(string cacheKey)
    {
        return (T)_cache.Get(cacheKey);
    }

    public virtual Task<T> GetAsync<T>(string cacheKey)
    {
        return Task.FromResult(Get<T>(cacheKey));
    }

    public virtual string Get(string cacheKey)
    {
        return _cache.Get(cacheKey) as string;
    }

    public virtual Task<string> GetAsync(string cacheKey)
    {
        return Task.FromResult(Get(cacheKey));
    }

    public virtual List<T> GetByPattern<T>(string pattern = "*")
    {
        var keys = GetKeysFromPattern(pattern);
        return keys.Select(Get<T>).ToList();
    }

    public virtual Task<List<T>> GetByPatternAsync<T>(string pattern = "*")
    {
        return Task.FromResult(GetByPattern<T>(pattern));
    }

    public virtual List<string> GetByPattern(string pattern = "*")
    {
        var keys = GetKeysFromPattern(pattern);
        return keys.Select(Get).ToList();
    }

    public virtual Task<List<string>> GetByPatternAsync(string pattern = "*")
    {
        return Task.FromResult(GetByPattern(pattern));
    }

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

    public virtual Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        return Task.Run(() => Set(cacheKey, data, absoluteExpiration, slidingExpiration));
    }

    public virtual void DeleteKeysByPattern(string pattern = "*")
    {
        var keysToDelete = GetKeysFromPattern(pattern);
        foreach (var key in keysToDelete) _cache.Remove(key);
    }

    public virtual Task DeleteKeysByPatternAsync(string pattern = "*")
    {
        return Task.Run(() => DeleteKeysByPattern(pattern));
    }

    public virtual void Remove(string cacheKey)
    {
        _cache.Remove(cacheKey);
        CacheKeys.Remove(cacheKey);
    }

    public virtual Task RemoveAsync(string cacheKey)
    {
        return Task.Run(() => Remove(cacheKey));
    }

    public virtual void Clear()
    {
        foreach (var key in CacheKeys) _cache.Remove(key);
    }

    public virtual Task ClearAsync()
    {
        return Task.Run(Clear);
    }

    private void ConsolidateKeys()
    {
        var invalidKeys = CacheKeys.Where(key => !_cache.TryGetValue(key, out _)).ToList();
        foreach (var key in invalidKeys) CacheKeys.Remove(key);
    }

    private void SetDefaults(IConfiguration configuration)
    {
        _defaultAbsoluteExpirationHours = int.Parse(configuration.GetSection("CacheConfiguration:InMemoryConfiguraton:TimeConfiguration:AbsoluteExpirationInHours")?.Value ?? "12");
        _defaultSlidingExpirationMinutes =
            int.Parse(configuration.GetSection("CacheConfiguration:InMemoryConfiguraton:TimeConfiguration:SlidingExpirationInMinutes")?.Value ?? "30");
        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_defaultAbsoluteExpirationHours),
            SlidingExpiration = TimeSpan.FromSeconds(_defaultSlidingExpirationMinutes)
        };
    }

    private IEnumerable<string> GetKeysFromPattern(string pattern = "*")
    {
        ConsolidateKeys();

        return pattern == "*" ? CacheKeys : CacheKeys.Where(_ => _.Contains(pattern));
    }
}