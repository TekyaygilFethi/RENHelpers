using Microsoft.EntityFrameworkCore;
using RENHelpers.ExampleProject.Database;
using RENHelpers.DataAccessHelpers.Extensions;

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
