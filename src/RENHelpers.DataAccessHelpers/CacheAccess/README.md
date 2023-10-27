# RENHelper - Cache Access

Welcome! This is the documentation of RENHelper Cache Access. RENHelper Cache Access is a library that streamlines and standardizes common development tasks, such as creating a Cache layer and implementing essential utility functions. It enables developers to adhere to best practices, save time, and focus on solving problems efficiently.

# Table of Contents
- [Cache Setup for .NET Projects](#cache-setup-for-net-projects)
- [REN Cache Helper](#ren-cache-helper)
  - [REN In Memory Cache Service](#ren-in-memory-cache-service)
    - [Standard Implementation of REN In Memory Cache Service](#standard-implementation-of-ren-in-memory-cache-service)
    - [Custom Implementation of REN In Memory Cache Service](#custom-implementation-of-ren-in-memory-cache-service)
    - [Using Both Overriding and Implementing Additional In Memory Cache Methods](#using-both-overriding-and-implementing-additional-in-memory-cache-methods)
  - [REN Redis Cache Service](#ren-redis-cache-service)
    - [Standard Implementation of REN Redis Cache Service](#standard-implementation-of-ren-redis-cache-service)
    - [Custom Implementation of REN Redis Cache Service](#custom-implementation-of-ren-redis-cache-service)
    - [Using Both Overriding and Implementing Additional In Memory Cache Methods](#using-both-overriding-and-implementing-additional-redis-cache-methods)

# Cache Setup for .NET Projects

Before we dive into fabulous pre-defined Cache Service implementations, we need to set up a cache setup connection in our project. REN supports 2 types of cache for now;

- In Memory
- Redis

First, you need to add some configuration to the appsetttings.json file:

```json
{
  "CacheConfiguration": {
    "RedisConfiguration": {
      "Url": "localhost:6379",
      "TimeConfiguration": {
        "AbsoluteExpirationInHours": 12
      },
      "DatabaseId": 0,
      "Username": "default",
      "Password": "mypwd",
      "AbortOnConnectFail": false
    },
    "InMemoryConfiguration": {
      "TimeConfiguration": {
        "AbsoluteExpirationInHours": 12,
        "SlidingExpirationInMinutes": 30
      }
    }
  }
}
```

Let us set up the both of them in order:

## Setting up the In Memory Cache

To set up In Memory cache you need to register required services into your
```Program.cs```

```csharp
builder.Services.AddMemoryCache();
```

## Setting up the Redis
To set up Redis cache you need to register required services into your Program.cs:

```csharp
builder.Services.AddDistributedMemoryCache();
```

# REN Cache Helper

All cache helpers inherit from ultimate interface called IRENCacheService. This interface ensures all the implemented cache services to achieve same operations and usage of SOLID! (Yes, SOLID rocks!)

Here is the content of IRENCacheService:
```csharp
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
```

## REN In Memory Cache Service

To use In Memory Cache, please make sure your appsettings.json content be as follows:
```json
{
  "CacheConfiguration": {
    "InMemoryConfiguration": {
      "TimeConfiguration": {
        "AbsoluteExpirationInHours": 12,
        "SlidingExpirationInMinutes": 30
      }
    }
  }
}
```

### Standard Implementation of REN In Memory Cache Service

RENInMemoryCacheService is a service that allows you to handle cache operations in memorily. Here is the standard implementation:

```csharp
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
        return Task.Factory.StartNew(()=>
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
```

Here, you can see all methods are defined as virtual which allows you to override them if necessary according to your needs. To use this pre-defined methods, you need register them in Program.cs file:

```csharp
builder.Services.RegisterRENCacheAccessHelpers<RENInMemoryCacheService>();
```

Here is the content of RegisterRENCacheAccessHelpers:

```csharp
public static IServiceCollection RegisterRENCacheAccessHelpers<T>(this IServiceCollection services) where T: ICacheService
{
    services.AddScoped(typeof(ICacheService), typeof(T));
    return services;
}
```
You can use IRENCacheService like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENRepository<User> _userRepository;
    private readonly IRENCacheService _cacheService;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IRENCacheService cacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _cacheService = cacheService;
    }

    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _cacheService.GetAsync<List<User>>(cacheKey);

        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _cacheService.SetAsync(cacheKey, users);

        return Ok(users);
    }
}
```

This code automatically uses in memory cache thanks to our registerations!


### Custom Implementation Of REN In Memory Cache Service

REN library does not like forcing you to do something with its own rules. That's why it allows you to customize it and make it fit to your own rules!

You can customize implementation of RENInMemoryCacheService via overriding its methods or implementing new ones to expand.

#### Overriding Existing Methods

As you can see in standard implementation, all methods are marked as virtual which means you can customize their content via overriding them.

You can override the existing methods to create a new class. To do that, newly created classes should inherit from RENInMemoryCacheService class:

```csharp
public class MyCacheService : RENInMemoryCacheService
{
    public MyCacheService(IMemoryCache cache) : base(cache) { }

    public override async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return await base.GetAsync<T>(cacheKey, cancellationToken);
    }
}
```

Here, we overrided the existing method to expand it's functionality. From now on, we have register MyCacheService in Program.cs instead of standard registeration:

```csharp
// builder.Services.RegisterRENCacheAccessHelpers<RENInMemoryCacheService>(); // SINCE WE ARE NOT USING STANDARD APPROACH ANYMORE
builder.Services.RegisterRENCacheAccessHelpers<MyCacheService>();
```

Since we are registering classes that inherits from IRENCacheService, we can use MyCacheService because we inherited from RENInMemoryCacheService which inherits from ICacheService interface!

And we may inject it to our classes as follows:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENCacheService _customCacheService;
    private readonly IRENRepository<User> _userRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IRENCacheService customCacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _customCacheService = customCacheService;
    }

    [HttpGet]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _customCacheService.GetAsync<List<User>>(cacheKey);

        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _customCacheService.SetAsync(cacheKey, users);

        return Ok(users);
    }
}
```

#### Implementing Additional Methods

Surely you should be able to implement new functions addition to existing one if you need it. This is SOLID after all! Let's see how we can do this. First you need to create the interface that inherits from IRENCache interface. Your new interface should contain additional methods and must be inherited from IRENCache interface to get all function signatures:

```csharp
public interface IMyCacheService: IRENCacheService
{
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default);
}
```

Then create your custom CacheService class and make it inherit from RENInMemoryCacheService class and your new interface (in this case it is IMyCacheService) that contains your custom function signature.

```csharp
public class MyCacheService : RENInMemoryCacheService, IMyCacheService
{
    public MyCacheService(IMemoryCache cache) : base(cache) { }
    
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting single custom...");
        // custom implementations
        return (await base.GetAsync<IEnumerable<T>>(cacheKey, cancellationToken)).SingleOrDefault(predicate);
    }
}
```

Then you have to change your register type in Program.cs to this since you will want to use IMyCacheService from now on:

```csharp
// builder.Services.RegisterRENCacheAccessHelpers<MyCacheService>();
builder.Services.AddScoped<IMyCacheService, MyCacheService>();
```

Then you can use this custom function as follows:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyCacheService _customCacheService;
    private readonly IRENRepository<User> _userRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IMyCacheService customCacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _customCacheService = customCacheService;
    }

    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> Index([FromQuery] int Id)
    {
        var cacheKey = $"users_{Id}";
        var user = await _customCacheService.GetAsync<User>(cacheKey);
        if (user != null) return Ok(user);

        var allUsersCacheKey = "users";
        user = await _customCacheService.GetSingleAsync<User>(allUsersCacheKey, _ => _.Id == Id);
        if (user != null) return Ok(user);

        user = await _userRepository.GetSingleAsync(_ => _.Id == Id);
        await _customCacheService.SetAsync(cacheKey, user);

        return Ok(user);
    }
}
```

#### Using Both Overriding and Implementing Additional In Memory Cache Methods

To use both you have to combine two methods. First create the IMyCacheService Interface to implement additional methods:

```csharp
public interface IMyCacheService: IRENCacheService
{
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T, bool> predicate, CancellationToken cancellationToken = default);
}
```

Then create MyCacheService class that inherits from RENInMemoryCacheService and  IMyCacheService and contains overriden method(s):
```csharp
public class MyCacheService : RENInMemoryCacheService, IMyCacheService
{
    public MyCacheService(IMemoryCache cache) : base(cache) { }

    public override async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return base.Get<T>(cacheKey);
    }

    // This method can share the same name as one of it's ancestor's functions
    // but since this method has different signatur it's okay!
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting single custom...");
        var result = await base.GetAsync<IEnumerable<T>>(cacheKey, cancellationToken);
        return result == null ? default : result.SingleOrDefault(predicate);
    }
}
```

In Program.cs we need to register MyCacheService class from interface IMyCacheService since it contains the additional method.

```csharp
builder.Services.AddScoped<IMyCacheService, MyCacheService>();
```

Then you can use your final cache service like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyCacheService _customCacheService;
    private readonly IRENRepository<User> _userRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IMyCacheService customCacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _customCacheService = customCacheService;
    }

    [HttpGet]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _customCacheService.GetAsync<List<User>>(cacheKey);
    
        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _customCacheService.SetAsync(cacheKey, users);
    
        return Ok(users);
    }

    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> Index([FromQuery] int Id)
    {
        var cacheKey = $"users_{Id}";
        var user = await _customCacheService.GetAsync<User>(cacheKey);
        if (user != null) return Ok(user);

        var allUsersCacheKey = "users";
        user = await _customCacheService.GetSingleAsync<User>(allUsersCacheKey, _ => _.Id == Id);
        if (user != null) return Ok(user);

        user = await _userRepository.GetSingleAsync(_ => _.Id == Id);
        await _customCacheService.SetAsync(cacheKey, user);

        return Ok(user);
    }
}
```

## REN Redis Cache Service

To use Redis Cache, please make sure your appsettings.json content be as follows:

```json
{
  "CacheConfiguration": {
    "RedisConfiguration": {
      "Url": "localhost:6379",
      "TimeConfiguration": {
        "AbsoluteExpirationInHours": 12
      },
      "DatabaseId": 0,
      "Username": "default",
      "Password": "my_pwd",
      "AbortOnConnectFail": false
    }
  }
}
```

### Standard Implementation of REN Redis Cache Service

RENRedisCacheService is a service that allows you to handle cache operations in redis. Here is the standard implementation:

```csharp
public class RENRedisCacheService : IRENCacheService
{
    private IDatabase _cacheDb;
    private ConnectionMultiplexer _connection;
    private int _defaultAbsoluteExpirationHours;

    protected RENRedisCacheService(IConfiguration configuration)
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

        if (username.IsValid())
            options.User = username;

        if (password.IsValid())
            options.Password = password;

        _connection = ConnectionMultiplexer.Connect(options);
        _cacheDb = _connection.GetDatabase();
    }
}
```

Here, you can see all methods are defined as virtual which allows you to override them if necessary according to your needs. To use this pre-defined methods, you need register them in Program.cs file:

```csharp
builder.Services.RegisterRENCacheAccessHelpers<RENRedisCacheService>();
```

Here is the content of RegisterRENCacheAccessHelpers:

```csharp
public static IServiceCollection RegisterRENCacheAccessHelpers<T>(this IServiceCollection services) where T: ICacheService
{
    services.AddScoped(typeof(ICacheService), typeof(T));
    return services;
}
```

You can use IRENCacheService like as follows:
```csharp
public class HomeController : ControllerBase
{
    private readonly IRENRepository<User> _userRepository;
    private readonly IRENCacheService _cacheService;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IRENCacheService cacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _cacheService = cacheService;
    }

    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _cacheService.GetAsync<List<User>>(cacheKey);

        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _cacheService.SetAsync(cacheKey, users);

        return Ok(users);
    }
}
```

### Custom Implementation Of REN Redis Cache Service

REN library does not like forcing you to do something with its own rules. That's why it allows you to customize it and make it fit to your own rules!

You can customize implementation of RENRedisCacheService via overriding its methods or implementing new ones to expand.

#### Overriding Existing Methods

As you can see in standard implementation, all methods are marked as virtual which means you can customize their content via overriding them.

You can override the existing methods to create a new class. To do that, newly created classes should inherit from RENRedisCacheService class:

```csharp
public class MyCacheService : RENRedisCacheService
{
    public MyCacheService(IConfiguration configuration) : base(configuration) { }

    public override T Get<T>(string cacheKey, CancellationToken cancellationToken = default);)
    {
        Console.WriteLine("Getting...");
        //custom implementations
        return base.Get<T>(cacheKey, cancellationToken);
    }
}
```

PLEASE BE CAREFUL about the constructors. In In Memory implementation RENInMemoryCacheService needed IConfiguration and IMemoryCache injeciton. Here, RENRedisCacheService needs IConfiguration injection!


Here, we overrided the existing method to expand it's functionality. From now on, we have register MyCacheService in Program.cs instead of standard registeration:

```csharp
// builder.Services.RegisterRENCacheAccessHelpers<RENInMemoryCacheService>(); // SINCE WE ARE NOT USING STANDARD APPROACH ANYMORE
builder.Services.RegisterRENCacheAccessHelpers<MyCacheService>();
```

Since we are registering classes that inherits from IRENCacheService, we can use MyCacheService because we inherited from RENRedisCacheService which inherits from ICacheService interface!

And we may inject it to our classes as follows:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENCacheService _customCacheService;
    private readonly IRENRepository<User> _customCacheService;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IRENCacheService cacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _cacheService = cacheService;
    }

    [HttpGet]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _cacheService.GetAsync<List<User>>(cacheKey);

        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _cacheService.SetAsync(cacheKey, users);

        return Ok(users);
    }
}
```

#### Implementing Additional Methods

Surely you should be able to implement new functions addition to existing one if you need it. This is SOLID after all! Let's see how we can do this. First you need to create the interface that inherits from IRENCache interface. Your new interface should contain additional methods and must be inherited from IRENCache interface to get all function signatures:

```csharp
public interface IMyCacheService: IRENCacheService
{
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default);
}
```

Then create your custom CacheService class and make it inherit from RENRedisCacheService class and your new interface (in this case it is IMyCacheService) that contains your custom function signature.

```csharp
public class MyCacheService : RENRedisCacheService, IMyCacheService
{
    public MyCacheService(IConfiguration configuration) : base(configuration) { }
    
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default);
    {
        Console.WriteLine("Getting single custom...");
        // custom implementations
        return (await base.GetAsync<IEnumerable<T>>(cacheKey, cancellationToken)).SingleOrDefault(predicate);
    }
}
```

Then you have to change your register type in Program.cs to this since you will want to use IMyCacheService from now on:

```csharp
// builder.Services.RegisterRENCacheAccessHelpers<MyCacheService>();
builder.Services.AddScoped<IMyCacheService, MyCacheService>();
```

Then you can use this custom function as follows:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyCacheService _customCacheService;
    private readonly IRENRepository<User> _userRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IMyCacheService customCacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _customCacheService = customCacheService;
    }

    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> Index([FromQuery] int Id)
    {
        var cacheKey = $"users_{Id}";
        var user = await _customCacheService.GetAsync<User>(cacheKey);
        if (user != null) return Ok(user);

        var allUsersCacheKey = "users";
        user = await _customCacheService.GetSingleAsync<User>(allUsersCacheKey, _ => _.Id == Id);
        if (user != null) return Ok(user);

        user = await _userRepository.GetSingleAsync(_ => _.Id == Id);
        await _customCacheService.SetAsync(cacheKey, user);

        return Ok(user);
    }
}
```

#### Using Both Overriding and Implementing Additional Redis Cache Methods

To use both you have to combine two methods. First create the IMyCacheService Interface to implement additional methods:

```csharp
public interface IMyCacheService: IRENCacheService
{
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T, bool> predicate, CancellationToken cancellationToken = default););
}
```

Then create MyCacheService class that inherits from RENRedisCacheService and  IMyCacheService and contains overriden method(s):

```csharp
public class MyCacheService : RENRedisCacheService, IMyCacheService
{
    public MyCacheService(IConfiguration configuration) : base(configuration) { }

    public override async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default);)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return base.Get<T>(cacheKey, cancellationToken);
    }

    // This method can share the same name as one of it's ancestor's functions
    // but since this method has different signatur it's okay!
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate, CancellationToken cancellationToken = default);)
    {
        Console.WriteLine("Getting single custom...");
        var result = await base.GetAsync<IEnumerable<T>>(cacheKey, cancellationToken);
        return result == null ? default : result.SingleOrDefault(predicate);
    }
}
```

In Program.cs we need to register MyCacheService class from interface IMyCacheService since it contains the additional method.

```csharp
builder.Services.AddScoped<IMyCacheService, MyCacheService>();
```

Then you can use your final cache service like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyCacheService _customCacheService;
    private readonly IRENRepository<User> _userRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow, IMyCacheService customCacheService)
    {
        _userRepository = uow.GetRENRepository<User>();
        _customCacheService = customCacheService;
    }

    [HttpGet]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        var cacheKey = "users";
        var users = await _customCacheService.GetAsync<List<User>>(cacheKey);
    
        if (users != null) return Ok(users);
        users = await _userRepository.GetListAsync();
        await _customCacheService.SetAsync(cacheKey, users);
    
        return Ok(users);
    }

    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> Index([FromQuery] int Id)
    {
        var cacheKey = $"users_{Id}";
        var user = await _customCacheService.GetAsync<User>(cacheKey);
        if (user != null) return Ok(user);

        var allUsersCacheKey = "users";
        user = await _customCacheService.GetSingleAsync<User>(allUsersCacheKey, _ => _.Id == Id);
        if (user != null) return Ok(user);

        user = await _userRepository.GetSingleAsync(_ => _.Id == Id);
        await _customCacheService.SetAsync(cacheKey, user);

        return Ok(user);
    }
}
```
