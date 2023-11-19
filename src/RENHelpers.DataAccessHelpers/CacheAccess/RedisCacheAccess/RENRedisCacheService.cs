using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RENHelpers.DataAccessHelpers.CacheHelpers;
using RENHelpers.StringHelper.ExtensionsFolder;
using StackExchange.Redis;

public class RENRedisCacheService : IRENCacheService
{
    private IDatabase _cacheDb;
    private ConnectionMultiplexer _connection;
    private int _defaultAbsoluteExpirationHours;

    public RENRedisCacheService(IConfiguration configuration)
    {
        SetDefaults(configuration);
        Connect(configuration);
    }

    /// <summary>
    /// Retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <returns>The cached T value.</returns>
    public virtual T Get<T>(string cacheKey)
    {
        var data = _cacheDb.StringGet(cacheKey);
        return data.HasValue ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    /// <summary>
    /// Asynchronously retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached T value.</returns>
    public virtual async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var data = await _cacheDb.StringGetAsync(cacheKey);
        return data.HasValue ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    /// <summary>
    /// Retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <returns>The cached string value.</returns>
    public virtual string Get(string cacheKey)
    {
        return _cacheDb.StringGet(cacheKey);
    }

    /// <summary>
    /// Asynchronously retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached string value.</returns>
    public virtual async Task<string> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _cacheDb.StringGetAsync(cacheKey);
    }

    /// <summary>
    /// Retrieves a list of data of type <typeparamref name="T"/> that matches the specified pattern.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached data items typed <typeparamref name="T"/>.</returns>
    public virtual List<T> GetByPattern<T>(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).Select(key => (string)key);
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
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Factory.StartNew(() => GetByPattern<T>(pattern), cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of string keys that match the specified pattern in the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached string keys.</returns>
    public virtual List<string> GetByPattern(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        return server.Keys(pattern: pattern).Select(key => (string)key).ToList();
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
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            return server.Keys(pattern: pattern).Select(key => (string)key).ToList();
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
        var expiration = GetAbsoluteExpiration(absoluteExpiration);

        _cacheDb.StringSet(cacheKey, JsonConvert.SerializeObject(data), expiration);
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
    public virtual async Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var expiration = GetAbsoluteExpiration(absoluteExpiration);
        await _cacheDb.StringSetAsync(cacheKey, JsonConvert.SerializeObject(data), expiration);
    }

    /// <summary>
    /// Deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    public virtual void DeleteKeysByPattern(string pattern = "*")
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        foreach (var key in keys) Remove((string)key);
    }

    /// <summary>
    /// Asynchronously deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    public virtual async Task DeleteKeysByPatternAsync(string pattern = "*", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        foreach (var key in keys) await RemoveAsync((string)key, cancellationToken);
    }

    /// <summary>
    /// Removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    public virtual void Remove(string cacheKey)
    {
        _cacheDb.KeyDelete(cacheKey);
    }

    /// <summary>
    /// Asynchronously removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    public virtual async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _cacheDb.KeyDeleteAsync(cacheKey);
    }

    /// <summary>
    /// Clears the database.
    /// </summary>
    public virtual void Clear()
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        server.FlushDatabase();
    }

    /// <summary>
    /// Asynchronously clears the database.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    public virtual async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
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

        if (username.RENIsValid())
            options.User = username;

        if (password.RENIsValid())
            options.Password = password;

        _connection = ConnectionMultiplexer.Connect(options);
        _cacheDb = _connection.GetDatabase();
    }
}