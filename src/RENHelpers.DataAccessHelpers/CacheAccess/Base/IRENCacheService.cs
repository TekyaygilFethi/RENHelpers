namespace RENHelpers.DataAccessHelpers.CacheHelpers;

public interface IRENCacheService
{
    /// <summary>
    /// Retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <returns>The cached T value.</returns>
    T Get<T>(string cacheKey);
    
    /// <summary>
    /// Asynchronously retrieves data of type <typeparamref name="T"/> from the cache.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="cacheKey">The key associated with the cached data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached T value.</returns>
    Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <returns>The cached string value.</returns>
    string Get(string cacheKey);
    
    /// <summary>
    /// Asynchronously retrieves a string value from the cache.
    /// </summary>
    /// <param name="cacheKey">The key associated with the cached string.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the cached string value.</returns>
    Task<string> GetAsync(string cacheKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a list of data of type <typeparamref name="T"/> that matches the specified pattern.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached data items typed <typeparamref name="T"/>.</returns>
    List<T> GetByPattern<T>(string pattern = "*");
    
    /// <summary>
    /// Asynchronously retrieves a list of data of type <typeparamref name="T"/> that matches the specified pattern.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing a list of cached data items typed <typeparamref name="T"/>.</returns>
    Task<List<T>> GetByPatternAsync<T>(string pattern = "*", CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a list of string keys that match the specified pattern in the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <returns>A list of cached string keys.</returns>
    List<string> GetByPattern(string pattern = "*");
    
    /// <summary>
    /// Asynchronously retrieves a list of string keys that match the specified pattern in the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match keys in the cache.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing a list of cached string keys.</returns>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<string>> GetByPatternAsync(string pattern = "*", CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets data of type <typeparamref name="T"/> in the cache with optional expiration settings.
    /// </summary>
    /// <typeparam name="T">The type of data to store in the cache.</typeparam>
    /// <param name="cacheKey">The key for storing the data in the cache.</param>
    /// <param name="data">The data to cache of type <typeparamref name="T"/>.</param>
    /// <param name="absoluteExpiration">The optional absolute expiration time for the cached data.</param>
    /// <param name="slidingExpiration">The optional sliding expiration time for the cached data.</param>
    void Set<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    
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
    Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    void DeleteKeysByPattern(string pattern = "*");
    
    /// <summary>
    /// Asynchronously deletes cache values by key pattern.
    /// </summary>
    /// <param name="pattern">The pattern for cache keys to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteKeysByPatternAsync(string pattern = "*", CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    void Remove(string cacheKey);
    
    /// <summary>
    /// Asynchronously removes cache value by key.
    /// </summary>
    /// <param name="cacheKey">The key to be deleted.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Clears the database.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Asynchronously clears the database.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);
}