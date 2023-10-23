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

Setting up the In Memory Cache
To set up In Memory cache you need to register required services into your
```Program.cs```

```csharp
builder.Services.AddMemoryCache();
```

Setting up the Redis
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
    private int _defaultAbsoluteExpirationHours;
    private int _defaultSlidingExpirationMinutes;
    private MemoryCacheEntryOptions _cacheOptions;

    protected RENInMemoryCacheService(IMemoryCache cache)
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
        foreach (var key in invalidKeys)
        {
            CacheKeys.Remove(key);
        }
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

    public override async Task<T> GetAsync<T>(string cacheKey)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return await base.GetAsync<T>(cacheKey);
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
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate);
}
```

Then create your custom CacheService class and make it inherit from RENInMemoryCacheService class and your new interface (in this case it is IMyCacheService) that contains your custom function signature.

```csharp
public class MyCacheService : RENInMemoryCacheService, IMyCacheService
{
    public MyCacheService(IMemoryCache cache) : base(cache) { }
    
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate)
    {
        Console.WriteLine("Getting single custom...");
        // custom implementations
        return (await base.GetAsync<IEnumerable<T>>(cacheKey)).SingleOrDefault(predicate);
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
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T, bool> predicate);
}
```

Then create MyCacheService class that inherits from RENInMemoryCacheService and  IMyCacheService and contains overriden method(s):
```csharp
public class MyCacheService : RENInMemoryCacheService, IMyCacheService
{
    public MyCacheService(IMemoryCache cache) : base(cache) { }

    public override async Task<T> GetAsync<T>(string cacheKey)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return base.Get<T>(cacheKey);
    }

    // This method can share the same name as one of it's ancestor's functions
    // but since this method has different signatur it's okay!
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate)
    {
        Console.WriteLine("Getting single custom...");
        var result = await base.GetAsync<IEnumerable<T>>(cacheKey);
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
      "Url": "redis-18003.c263.us-east-1-2.ec2.cloud.redislabs.com:18003",
      "TimeConfiguration": {
        "AbsoluteExpirationInHours": 12
      },
      "DatabaseId": 0,
      "Username": "default",
      "Password": "ofqa9YpQ6iw5tmDsoH5EW1OTMJtKs2Gs",
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

    public override T Get<T>(string cacheKey)
    {
        Console.WriteLine("Getting...");
        //custom implementations
        return base.Get<T>(cacheKey);
    }
}
```

PLEASE BE CAREFUL about the constructors. In In Memory implementation RENInMemoryCacheService needed IMemoryCache injeciton. Here, RENRedisCacheService needs IConfiguration injection!


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
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate);
}
```

Then create your custom CacheService class and make it inherit from RENRedisCacheService class and your new interface (in this case it is IMyCacheService) that contains your custom function signature.

```csharp
public class MyCacheService : RENRedisCacheService, IMyCacheService
{
    public MyCacheService(IConfiguration configuration) : base(configuration) { }
    
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate)
    {
        Console.WriteLine("Getting single custom...");
        // custom implementations
        return (await base.GetAsync<IEnumerable<T>>(cacheKey)).SingleOrDefault(predicate);
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
    Task<T> GetSingleAsync<T>(string cacheKey, Func<T, bool> predicate);
}
```

Then create MyCacheService class that inherits from RENRedisCacheService and  IMyCacheService and contains overriden method(s):

```csharp
public class MyCacheService : RENRedisCacheService, IMyCacheService
{
    public MyCacheService(IConfiguration configuration) : base(configuration) { }

    public override async Task<T> GetAsync<T>(string cacheKey)
    {
        Console.WriteLine("Getting custom...");
        //custom implementations
        return base.Get<T>(cacheKey);
    }

    // This method can share the same name as one of it's ancestor's functions
    // but since this method has different signatur it's okay!
    public async Task<T> GetSingleAsync<T>(string cacheKey, Func<T,bool> predicate)
    {
        Console.WriteLine("Getting single custom...");
        var result = await base.GetAsync<IEnumerable<T>>(cacheKey);
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