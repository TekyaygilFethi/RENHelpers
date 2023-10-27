# RENHelper - Database Access

Welcome! This is the documentation of RENHelper Database Access. RENHelper Database Access is a library that streamlines and standardizes common development tasks, such as creating a Cache layer and implementing essential utility functions. It enables developers to adhere to best practices, save time, and focus on solving problems efficiently. For now, it only works with Entity Framework!

# Table of Contents
- [Database Setup for .NET Projects](#database-setup-for-net-projects)
- [REN Unit Of Work Helper](#ren-unit-of-work-helper)
  * [Standard Implementation of RENUnitOfWork](#standard-implementation-of-renunitofwork)
  * [Custom Implementation of RENUnitOfWork](#custom-usage-of-renunitofwork)
  * [Using Both Overriding and Implementing Additional Unit Of Work Methods](#using-both-overriding-and-implementing-additional-unit-of-work-methods)
- [REN Repository Helper](#ren-repository-helper)
  * [Standard Implementation of RENRepository](#standard-implementation-of-renrepository)
  * [Custom Implementation of RENRepository](#custom-implementation-of-renrepository)
  * [Using Both Overriding and Implementing Additional Repository Methods](#using-both-overriding-and-implementing-additional-repository-methods)

# Database Setup for .NET Projects

```csharp
public class RENDbContext: DbContext
{
    public RENDbContext(DbContextOptions options):base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Side> Sides { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUserEntities(modelBuilder);
    }
    
    private void ConfigureUserEntities(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasOne(_ => _.Side)
            .WithMany(_=>_.Users)
            .HasForeignKey(_=>_.SideId);
    }
}
```

In this example I used Entity Framework Code First approach which means User and Side classes are POCO Class:

```csharp
public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Surname { get; set; }
    public Side Side { get; set; }
    public int SideId { get; set; }
}

public class Side
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; }
}
```

In your appsettings.json file you need to set proper Connection Strings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Dev": "Server=MYSERVER;Database=RENTestDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Then in your ```Program.cs```, you have the register your DbContext.
Please note that if your json level of connectionstrings are different, you need to change the GetSection("
differedpath") parameter accordingly:

```csharp
builder.Services.AddDbContext<RENDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Dev").Value);
});
```

Please note that in this example we used Sql Server but you can change the database according to your needs!
Then in Package Manager Console execute these commands to create your migration and apply it to database:

```bash
>> Add-Migration Migration_1
>> Update-Databse -Verbose
```

or alternatively, you can execute following commands in terminal:

```bash
>> dotnet ef migrations add Migration_1
>> dotnet ef database update
```

When creating the database is done you are good to go to use REN Database Access Helpers!

# REN Unit Of Work Helper

A Unit of Work is a design pattern used in software development, primarily in the context of working with databases and
managing transactions. It is often used in conjunction with the Repository pattern to help manage data access and
transactions in a consistent and efficient way. The main goal of the Unit of Work pattern is to ensure that all database
operations within a given unit of work are performed atomically, either all succeeding or all failing.

You can have get information through here:
​https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application

```RENUnitOfWork``` section has the standard implementations of UnitOfWork pattern which can be expandable or overriden.

## Standard Implementation of RENUnitOfWork

Here is the standard implementation of ```RENUnitOfWork``` which allows you to use pre-defined UnitOfWork:

```csharp
public interface IRENUnitOfWork<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    ///     Commits the changes made in the unit of work to the database.
    /// </summary>
    void SaveChanges();

    /// <summary>
    ///     Commits the changes made in the unit of work to the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>An instance of the repository for the specified entity type.</returns>
    IRENRepository<TEntity>? GetRENRepository<TEntity>() where TEntity : class;

    /// <summary>
    ///     Gets a repository for a specific entity type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>A task representing an instance of the repository for the specified entity type.</returns>
    Task<IRENRepository<TEntity>?> GetRENRepositoryAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;
}

/// <summary>
///     Represents a unit of work for managing database transactions and repositories in a generic context.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class RENUnitOfWork<TDbContext> : IRENUnitOfWork<TDbContext> where TDbContext : DbContext
{
    protected readonly TDbContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RENUnitOfWork{TDbContext}" /> class.
    /// </summary>
    /// <param name="context">The database context to be used within the unit of work.</param>
    public RENUnitOfWork(TDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(TDbContext));
    }

    /// <summary>
    ///     Commits the changes made in the unit of work to the database.
    /// </summary>
    public virtual void SaveChanges()
    {
        using var ctxTransaction = _context.Database.BeginTransaction();
        try
        {
            _context.SaveChanges();
            ctxTransaction.Commit();
        }
        catch (Exception)
        {
            ctxTransaction.Rollback();
            throw;
        }
    }

    /// <summary>
    ///     Commits the changes made in the unit of work to the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var ctxTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await ctxTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await ctxTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    ///     Gets a repository for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>An instance of the repository for the specified entity type.</returns>
    public virtual IRENRepository<TEntity>? GetRENRepository<TEntity>() where TEntity : class
    {
        return (IRENRepository<TEntity>?)Activator.CreateInstance(typeof(RENRepository<TEntity>), _context);
    }

    /// <summary>
    ///     Gets a repository for a specific entity type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository.</typeparam>
    /// <returns>A task representing an instance of the repository for the specified entity type.</returns>
    public virtual Task<IRENRepository<TEntity>?> GetRENRepositoryAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return (IRENRepository<TEntity>?)Activator.CreateInstance(typeof(RENRepository<TEntity>), _context);
        }, cancellationToken);
    }
}
```

Here, you can see all methods are defined as virtual which allows you to override them if necessary according to your
needs. To use this pre-defined methods, you need register them in ``` file:

```csharp
builder.Services.RegisterRENDatabaseAccessHelpers()
```

Here, the content of the RegisterRENDatabaseAccessHelpers is:

```csharp
public static IServiceCollection RegisterRENDatabaseAccessHelpers(this IServiceCollection services)
{
    services.AddScoped(typeof(IRENUnitOfWork<>), typeof(RENUnitOfWork<>));
    
    return services;
}
```

As you can see, only the ```RENUnitOfWork``` is registered. Since we will instantiate all repositories
from ```RENUnitOfWork```, we are not registering them through here.
Then you are good to go! You can use standard ```RENUnitOfWork``` like this:

```csharp
public class UserService
{
    private IRENUnitOfWork<RENDbContext> _uow;

    public HomeController(IRENUnitOfWork<RENDbContext> uow)
    {
        _uow = uow;
    }
}
```

You are good to go! Let's examine the Repository part and expand the usage of UnitFoWork with its full potential!

## Custom Implementation of RENUnitOfWork

REN library does not like forcing you to do something with its own rules. That's why it allows you to customize it and
make it fit to your own rules!
You can customize implementation of ```RENUnitOfWork``` via overriding its methods or implementing new ones to expand.

### Overriding Existing Methods

As you can see in standard implementation, all methods are marked as virtual which means you can customize their content
via overriding them.
You can override the existing methods to create a new class. To do that, newly created classes should inherit
from ```RENUnitOfWork``` class:

```csharp
public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }

    public override IRENRepository<TEntity>? GetRENRepository<TEntity>()
    {
        // Make custom actions here
        return base.GetRENRepository<TEntity>();
    }
    
    // we didn't override SaveChanges() method because we don't want to change it's behaviour
}
```

```
Here, we use T is RENDbContext because RENDbContext is already DbContext!
```

Here, we overrided the existing method to expand it's functionality. From now on, we have register MyUnitOfWork in
Program.cs instead of standard registeration:

```csharp
// builder.Services.RegisterRENDatabaseAccessHelpers(); // SINCE WE ARE NOT USING STANDARD APPROACH ANYMORE
builder.Services.AddScoped(typeof(IRENUnitOfWork<>), typeof(MyUnitOfWork<>));
```

And we may inject it to our classes:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENUnitOfWork<RENDbContext> _uow;
    private readonly IRENRepository<User> _customUserRepository;

    public HomeController(IRENUnitOfWork<RENDbContext> uow)
    {
        _uow = uow;
        _customUserRepository = uow.GetRENRepository<User>();
    }
}
```

Here, you will use your MyUnitOfWork class thanks to registeration.

### Implementing Additional Methods

Surely you should be able to implement new functions addition to existing one if you need it. This is SOLID after all!
Let's see how we can do this. First you need to create the interface that inherits from ```IRENUnitOfWork``` interface.
Your new interface should contain additional methods and must be inherited from ```IRENUnitOfWork``` interface to get
all function signatures:

```csharp
public interface IMyUnitOfWork<TDbContext>: IRENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    Task MyCustomFunction(CancellationToken cancellationToken = default);
}
```

Then create your custom UnitOfWork class and make it inherit from ```RENUnitOfWork``` class and your new interface (in
this case it is IMyUnitOfWork) that contains your custom function signature.

```csharp
public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext>, IMyUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }

    public Task MyCustomFunction(CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("This is my custom Function");
            // other custom implementations!

        }, cancellationToken);
    }
}
```

Then you have to change your register type in Program.cs to this since you will want to use ```IMyUnitOfWork``` from now
on:

```csharp
builder.Services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
```

Then you can use this custom function as follows:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyUnitOfWork<RENDbContext> _uow;

    public HomeController(IMyUnitOfWork<RENDbContext> uow)
    {
        _uow = uow;
    }

    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        await _uow.MyCustomFunction();
        return Ok();
    }   
}
```

### Using Both Overriding and Implementing Additional Unit Of Work Methods

To use both you have to combine two methods. First create your interface as shown
in ```Implementing Additional Methods``` section.

```csharp
public interface IMyUnitOfWork<TDbContext>: IRENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    Task MyCustomFunction(CancellationToken cancellationToken = default);
}
```

Then, create your custom UnitOfWork class that inherits from RENUnitOfWork (since we want to overrride the desired
methods and don't want to implement other methods again that IRENUnitOfWork contains) and IMyUnitOfWork (since we want
to get newly created custom function signature).

```csharp
public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext>, IMyUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }

    public Task MyCustomFunction(CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("This is my custom Function");
            // other custom implementations!
        }, cancellationToken);
    }

    public override Task<bool> SaveChangesAsync()
    {
        Console.WriteLine("This is my custom SaveChangesAsync");
        return base.SaveChangesAsync();
    }
}
```

In ```Program.cs``` you have to register ```MyUnitOfWork```:

```csharp
builder.Services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
```

And you can use it like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyUnitOfWork<RENDbContext> _uow;

    public HomeController(IMyUnitOfWork<RENDbContext> uow)
    {
        _uow = uow;
    }

    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        await _uow.MyCustomFunction();
        return Ok();
    }
        
    [HttpGet, Route("InsertSide")]
    public async Task<ActionResult> InsertSide([FromQuery] string side)
    {
        await _customSideRepository.InsertAsync(new Side()
        {
            Name = side    
        });
            
        await _uow.SaveChangesAsync();
        return Ok();
    }
        
    [HttpGet, Route("InsertUser")]
    public async Task<ActionResult> InsertUser([FromQuery] string name, string surname, int sideId)
    {
        _userRepository.Insert(new User()
        {
            Name = name,
            Surname = surname,
            SideId = sideId
        });
            
        _uow.SaveChanges();
        return Ok();
    }
}
```

Here, we used both overriden (SaveChangesAsync()) and extra implemented(MyCustomFunction()) functions!

# REN Repository Helper

The Repository Pattern is a design pattern commonly used in software development to separate the logic that retrieves
data from a data source (such as a database) from the rest of the application. It provides an abstraction layer between
the application code and the data source, promoting a more modular and maintainable design.
You can have get information through
here: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
RENRepository section has the standart implementations of Repository pattern which can be expandable or overriden.

## Standard Implementation of RENRepository

Here is the standard implementation of RENRepository which allows you to use pre-defined Repository:

```csharp
public interface IRENRepository<TEntity> where TEntity : class
{
    #region Create

    /// <summary>
    ///     Inserts a single entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    void Insert(TEntity entity);

    /// <summary>
    ///     Inserts a single entity into the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Inserts a list of entities into the repository.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    void Insert(List<TEntity> entities);

    /// <summary>
    ///     Inserts a list of entities into the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to insert asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Read

    /// <summary>
    ///     Gets a queryable collection of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a queryable collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the queryable collection of entities.</returns>
    Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A list of entities.</returns>
    List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false);

    /// <summary>
    ///     Gets a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the list of entities.</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a single entity from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A single entity or null if not found.</returns>
    TEntity? GetSingle(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false);

    /// <summary>
    ///     Gets a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the single entity or null if not found.</returns>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    ///     Updates a single entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    ///     Updates a single entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>
    ///     Deletes a single entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    ///     Deletes a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a list of entities from the repository.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    void Delete(List<TEntity> entities);

    /// <summary>
    ///     Deletes a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion
}

public class RENRepository<TEntity> : IRENRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RENRepository{TEntity}" /> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RENRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException("context");
        _dbSet = context.Set<TEntity>();
    }

    #region Create

    /// <summary>
    ///     Inserts a single entity into the database.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    public virtual void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <summary>
    ///     Inserts a single entity into the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to insert asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Inserts a list of entities into the database.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    public virtual void Insert(List<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /// <summary>
    ///     Inserts a list of entities into the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to insert asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    #endregion

    #region Read

    /// <summary>
    ///     Gets a queryable collection of entities from the repository.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A queryable collection of entities.</returns>
    public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        IQueryable<TEntity> query = _dbSet;
        include?.Invoke(query);

        if (isReadOnly)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        return orderBy != null ? orderBy(query) : query;
    }

    /// <summary>
    ///     Gets a queryable collection of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the queryable collection of entities.</returns>
    public virtual Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            IQueryable<TEntity> query = _dbSet;
            include?.Invoke(query);

            if (isReadOnly)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return orderBy != null ? orderBy(query) : query;
        }, cancellationToken);
    }

    /// <summary>
    ///     Gets a list of entities from the database.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A list of entities.</returns>
    public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false)
    {
        return GetQueryable(filter, orderBy, include, isReadOnly).ToList();
    }

    /// <summary>
    ///     Gets a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the list of entities.</returns>
    public virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false, CancellationToken cancellationToken = default)
    {
        return GetQueryableAsync(filter, orderBy, include, isReadOnly, cancellationToken).Result.ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Gets a single entity from the database.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <returns>A single entity or null if not found.</returns>
    public virtual TEntity? GetSingle(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false)
    {
        return GetQueryable(filter, orderBy, include, isReadOnly).SingleOrDefault();
    }

    /// <summary>
    ///     Gets a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression.</param>
    /// <param name="orderBy">An optional ordering expression.</param>
    /// <param name="include">An optional include expression.</param>
    /// <param name="isReadOnly">A flag indicating if the query is read-only.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the single entity or null if not found.</returns>
    public virtual Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null,
        bool isReadOnly = false,
        CancellationToken cancellationToken = default)
    {
        return GetQueryableAsync(filter, orderBy, include, isReadOnly, cancellationToken).Result.SingleOrDefaultAsync(cancellationToken);
    }

    #endregion
    
    #region Update

    /// <summary>
    ///     Updates a single entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public virtual void Update(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    ///     Updates a single entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Entry(entity).State = EntityState.Modified;
        }, cancellationToken);
    }

    #endregion

    #region Delete

    /// <summary>
    ///     Deletes a single entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    ///     Deletes a single entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _dbSet.Remove(entity);
        }, cancellationToken);
    }

    /// <summary>
    ///     Deletes a list of entities from the database.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    public virtual void Delete(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    ///     Deletes a list of entities from the repository asynchronously.
    /// </summary>
    /// <param name="entities">The list of entities to delete asynchronously.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task DeleteAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            _dbSet.RemoveRange(entities);
        }, cancellationToken);
    }

    #endregion
}
```

Here, you can see all methods are defined as virtual which allows you to override them if necessary according to your
needs. To use this pre-defined methods, you don't need to register them in Program.cs file since they can be produced
within the ```GetRENRepository``` function in ```RENUnitOfWork```
You can use ```RENRepositories``` like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENRepository<User> _userRepository;
    
    public HomeController(IMyUnitOfWork<RENDbContext> uow)
    {
        _userRepository = _uow.GetRENRepository<User>();
    }
    
    [HttpGet, Route("GetUsers")]
    public async Task<IActionResult> SideIndex()
    {
        var users = await _userRepository.GetGetLisAsync();

        return Ok(users);
    }
}
```

## Custom Implementation of RENRepository

You can customize implementation of RENRepository via overriding its methods or implementing new ones to expand.

### Overriding Existing Methods

As you can see in standard implementation, all methods are marked as virtual which means you can customize their content
via overriding them.
You can override the existing methods to create a new class. To do that, newly created classes should inherit
from ```RENRepository``` class:

```csharp
public class MyRepository<TEntity> : RENRepository<TEntity> where TEntity : class
{
    public MyRepository(RENDbContext context) : base(context) { }

    public override Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting......");
        // Custom implementations
        return base.GetSingleAsync(filter, orderBy, include, isReadOnly, cancellationToken);
    }
}
```

Here, we overrided the existing method to expand it's functionality. From now on, we don't have register MyRepository in
Program.cs since we will use it in our UnitOfWork class.
In Program.cs you must register it with IRENUnitOfWork since we are not using custom Interface here:

```csharp
builder.Services.AddScoped(typeof(IRENUnitOfWork<>), typeof(MyUnitOfWork<>));
```

To achieve this we can create new function named GetMyRepository in our UnitOfWork:

```csharp
public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }
    
    public override IRENRepository<TEntity>? GetRENRepository<TEntity>()
    {
        return (IRENRepository<TEntity>?)Activator.CreateInstance(typeof(MyRepository<TEntity>), new object[] { _context });
    }
}
```

From now on GetMyRepository function returns IMyRepository object which contains our custom implementation.

```
You can implement custom function instead of overriding existing one to get repository and use it. It is all up to you!
```

And you can get and use your custom repository like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IRENUnitOfWork<RENDbContext> _uow;
    private readonly IRENRepository<Side> _customSideRepository;
    
    public HomeController(IRENUnitOfWork<RENDbContext> uow)
    {
        _uow = uow;
        _customSideRepository = _uow.GetRENRepository<Side>();
    }
    
    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        var side = await _customSideRepository.GetSingleAsync(_=>_.Id == 1);
        return Ok(side);
    }
}
```

### Implementing Additional Methods

Surely you should be able to implement new functions addition to existing one if you need it. This is SOLID after all!
Let's see how we can do this. First you need to create the interface that inherits from IRENRepository interface. Your
new interface should contain additional methods:

```csharp
public interface IMyRepository<TEntity> : IRENRepository<TEntity> where TEntity : class
{
    Task MyCustomFunction(CancellationToken cancellationToken = default);
}
```

Then create your ```MyRepository``` class that inherits from ```IMyRepository```:

```csharp
public class MyRepository<TEntity> : RENRepository<TEntity>, IMyRepository<TEntity> where TEntity : class
{
    public MyRepository(RENDbContext context) : base(context) { }

    public Task MyCustomFunction(CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("This is my custom Function");
            // other custom implementations!

        }, cancellationToken);
    }
}
```

```
Since we created new interface and want to use it, we can not override the GetRENRepository function because we want a function that returns our newly created interface.
```

Customize our UnitOfWork class to return our newly created interface:

```csharp
public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext>, IMyUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }
    
    public IMyRepository<TEntity>? GetMyRepository<TEntity>() where TEntity: class
    {
        return (IMyRepository<TEntity>?)Activator.CreateInstance(typeof(MyRepository<TEntity>), new object[] { _context });
    }
}
```

Also update the interface:

```csharp
public interface IMyUnitOfWork<TDbContext>: IRENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    IMyRepository<TEntity>? GetMyRepository<TEntity>() where TEntity : class;
}
```

Register your custom ```UnitOfWork``` class in your Program.cs:

```csharp
builder.Services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
```

And you can get and use your custom repository like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyRepository<User> _userRepository;

    public HomeController(IMyUnitOfWork<RENDbContext> uow)
    {
        _userRepository = uow.GetMyRepository<User>();
    }
    
    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        await _userRepository.MyCustomFunction();
        return Ok();
    }
}
```

### Using Both Overriding and Implementing Additional Repository Methods

To use both you have to combine two methods. First create the IMyRepository Interface to implement additional methods:

```csharp
public interface IMyRepository<TEntity> : IRENRepository<TEntity> where TEntity : class
{
    Task MyCustomFunction(CancellationToken cancellationToken = default);
}
```

Then create ```MyRepository``` class that inherits from ```RENRepository``` and  ```IMyRepository``` and contains
overriden method(s):

```csharp
public class MyRepository<TEntity> : RENRepository<TEntity>, IMyRepository<TEntity> where TEntity : class
{
    public MyRepository(RENDbContext context) : base(context) { }

    public override Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Action<IQueryable<TEntity>> include = null, bool isReadOnly = false, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Getting custom async......");
        // Other custom implementations
        return base.GetSingleAsync(filter, orderBy, include, isReadOnly, cancellationToken);
    }

    public Task MyCustomFunction(CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("This is my custom Function");
            // other custom implementations!

        }, cancellationToken);
    }
}
```

To use ```MyRepository``` class with the interface, we created ```IMyRepository``` interface. We need to add a
repository getter function to get repositories in ```IMyRepository``` type instead of ```IRENRepository```. We need that
because ```IRENRepository``` does not contain our custom function!

```csharp
public interface IMyUnitOfWork<TDbContext>: IRENUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    IMyRepository<TEntity>? GetMyRepository<TEntity>() where TEntity : class;
}

public class MyUnitOfWork<TDbContext> : RENUnitOfWork<TDbContext>, IMyUnitOfWork<TDbContext> where TDbContext : RENDbContext
{
    public MyUnitOfWork(TDbContext context) : base(context) { }

    public IMyRepository<TEntity>? GetMyRepository<TEntity>() where TEntity: class
    {
        return (IMyRepository<TEntity>?)Activator.CreateInstance(typeof(MyRepository<TEntity>), new object[] { _context });
    }
}
```

In ```Program.cs``` we need to register ```MyUnitOfWork``` class from interface ```IMyUnitOfWork``` since it contains
the repository getter method.

```csharp
builder.Services.AddScoped(typeof(IMyUnitOfWork<>), typeof(MyUnitOfWork<>));
```

Then you can use your final Repository and Unit Of Work classes like this:

```csharp
public class HomeController : ControllerBase
{
    private readonly IMyRepository<User> _customUserRepository;
    private readonly ICacheService _cacheService;

    public HomeController(IMyUnitOfWork<RENDbContext> uow)
    {
        _customUserRepository = uow.GetMyRepository<User>();
    }

    [HttpGet, Route("Index")]
    public async Task<IActionResult> Index()
    {
        await _customUserRepository.MyCustomFunction(); // our custom function
        await _customUserRepository.GetSingleAsync(_ => _.Id == 1); // our overriden function
        return Ok();
    }
}
```

## Documentation

[Documentation](https://fethis-organization.gitbook.io/ren-regular-everyday-normal-helper/)

