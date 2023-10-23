namespace RENHelpers.DataAccessHelpers.CacheAccess.Base;

public interface IRENCacheService
{
    T Get<T>(string cacheKey);
    Task<T> GetAsync<T>(string cacheKey);
    string Get(string cacheKey);
    Task<string> GetAsync(string cacheKey);
    List<T> GetByPattern<T>(string pattern = "*");
    Task<List<T>> GetByPatternAsync<T>(string pattern = "*");
    List<string> GetByPattern(string pattern = "*");
    Task<List<string>> GetByPatternAsync(string pattern = "*");
    void Set<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    Task SetAsync<T>(string cacheKey, T data, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    void DeleteKeysByPattern(string pattern = "*");
    Task DeleteKeysByPatternAsync(string pattern = "*");
    void Remove(string cacheKey);
    Task RemoveAsync(string cacheKey);
    void Clear();
    Task ClearAsync();
}