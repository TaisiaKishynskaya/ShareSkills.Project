using StackExchange.Redis;

namespace App.Services.Concrete;

public static class RedisServiceExtensions
{
    public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetSection("Redis")["ConnectionString"];
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
    }
}