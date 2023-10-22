Database Setup for .NET Projects

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
Then in your Program.cs, you have the register your DbContext. 
Please note that if your json level of connectionstrings are different, you need to change the GetSection("differedpath") parameter accordingly:
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
## Documentation

[Documentation](https://fethis-organization.gitbook.io/ren-regular-everyday-normal-helper/ren.dataaccesshelpers/database-helpers)

