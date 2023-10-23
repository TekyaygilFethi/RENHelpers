using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RENHelpers.StringHelper.ExtensionsFolder;
using StackExchange.Redis;

namespace RENHelpers.DataAccessHelpers.CacheHelpers;

public class RENRedisCacheService : IRENCacheService
{
    private IDatabase _cacheDb;
    private DistributedCacheEntryOptions _cacheOptions;
    private ConnectionMultiplexer _connection;
    private int _defaultAbsoluteExpirationHours;

    protected RENRedisCacheService(IConfiguration configuration)
    {
        SetDefaults(configuration);
        Connect(configuration);
    }

    public virtual T Get<T>(string cacheKey)
    {
        var data = _cacheDb.StringGet(cacheKey);
        return data.HasValue ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    public virtual async Task<T> GetAsync<T>(string cacheKey)
    {
        var data = await _cacheDb.StringGetAsync(cacheKey);
        return data.HasValue ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    public virtual string Get(string cacheKey)
    {
        return _cacheDb.StringGet(cacheKey);
    }

    public virtual async Task<string> GetAsync(string cacheKey)
    {
        return await _cacheDb.StringGetAsync(cacheKey);
    }

    public virtual List<T> GetByPattern<T>(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).Select(key => (string)key);
        return keys.Select(Get<T>).ToList();
    }

    public virtual Task<List<T>> GetByPatternAsync<T>(string pattern = "*")
    {
        return Task.FromResult(GetByPattern<T>(pattern));
    }

    public virtual List<string> GetByPattern(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        return server.Keys(pattern: pattern).Select(key => (string)key).ToList();
    }

    public virtual async Task<List<string>> GetByPatternAsync(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        return server.Keys(pattern: pattern).Select(key => (string)key).ToList();
    }

    public virtual void Set<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var expiration = GetAbsoluteExpiration(absoluteExpiration);

        _cacheDb.StringSet(cacheKey, JsonConvert.SerializeObject(data), expiration);
    }

    public virtual async Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var expiration = GetAbsoluteExpiration(absoluteExpiration);

        await _cacheDb.StringSetAsync(cacheKey, JsonConvert.SerializeObject(data), expiration);
    }

    public virtual void DeleteKeysByPattern(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        foreach (var key in keys) Remove((string)key);
    }

    public virtual async Task DeleteKeysByPatternAsync(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        foreach (var key in keys) await RemoveAsync((string)key);
    }

    public virtual void Remove(string cacheKey)
    {
        _cacheDb.KeyDelete(cacheKey);
    }

    public virtual async Task RemoveAsync(string cacheKey)
    {
        await _cacheDb.KeyDeleteAsync(cacheKey);
    }

    public virtual void Clear()
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        server.FlushDatabase();
    }

    public virtual async Task ClearAsync()
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        await server.FlushDatabaseAsync();
    }

    private TimeSpan GetAbsoluteExpiration(TimeSpan? absoluteExpiration = null)
    {
        return absoluteExpiration ?? TimeSpan.FromHours(_defaultAbsoluteExpirationHours);
    }

    private void SetDefaults(IConfiguration configuration)
    {
        _defaultAbsoluteExpirationHours = int.Parse(configuration.GetSection("CacheConfiguration:RedisConfiguraton:TimeConfiguration:AbsoluteExpirationInHours")?.Value ?? "12");
    }

    private void Connect(IConfiguration configuration)
    {
        var username = configuration.GetSection("CacheConfiguration:RedisConfiguration:Username")?.Value;
        var password = configuration.GetSection("CacheConfiguration:RedisConfiguration:Password")?.Value;
        var url = configuration.GetSection("CacheConfiguration:RedisConfiguration:Url")?.Value;
        var databaseId = int.Parse(configuration.GetSection("CacheConfiguration:RedisConfiguration:DatabaseId")?.Value);
        var abortOnConnectFail = bool.Parse(configuration.GetSection("CacheConfiguration:RedisConfiguration:AbortOnConnectFail")?.Value);

        var options = new ConfigurationOptions
        {
            EndPoints = { url },
            DefaultDatabase = databaseId,
            AbortOnConnectFail = abortOnConnectFail
        };

        if (username.IsValid())
            options.User = username;

        if (password.IsValid())
            options.Password = password;

        _connection = ConnectionMultiplexer.Connect(options);
        _cacheDb = _connection.GetDatabase();
    }
}