using Microsoft.EntityFrameworkCore;
using RENHelpers.DataAccessHelpers.Extensions;
using RENHelpers.ExampleProject.Database;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddMemoryCache();

#region Db
builder.Services.AddDbContext<RENDbContext>(options => { options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Dev").Value); });
#endregion

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var redisOptions = new ConfigurationOptions
    {
        EndPoints = { configuration.GetSection("CacheConfiguration:RedisConfiguration:Url")?.Value },
        DefaultDatabase = int.Parse(configuration.GetSection("CacheConfiguration:RedisConfiguration:DatabaseId")?.Value ?? "0"),
        AbortOnConnectFail = bool.Parse(configuration.GetSection("CacheConfiguration:RedisConfiguration:AbortOnConnectFail")?.Value),
        User = configuration.GetSection("CacheConfiguration:RedisConfiguration:Username")?.Value,
        Password = configuration.GetSection("CacheConfiguration:RedisConfiguration:Password")?.Value
    };
    return ConnectionMultiplexer.Connect(redisOptions);
});

builder.Services.RegisterRENDatabaseAccessHelpers();
builder.Services.RegisterRENCacheAccessHelpers<RENRedisCacheService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
