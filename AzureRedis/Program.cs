using AzureRedis.Interface;
using AzureRedis.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// One shared Redis connection manager for this application instance.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string connectionString =
        sp.GetRequiredService<IConfiguration>()
          .GetConnectionString("Redis")
        ?? throw new InvalidOperationException(
            "Redis connection string is missing.");

    return ConnectionMultiplexer.Connect(connectionString);
});

// Register our Redis wrapper.
builder.Services.AddSingleton<IRedisCache, RedisCache>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
