using App.Services.Abstract;
using StackExchange.Redis;
using System.Text.Json;

namespace App.Services.Concrete;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redisConnection;

    public RedisCacheService(IConnectionMultiplexer redisConnection)
    {
        _redisConnection = redisConnection;
    }
    
    public async Task SetCacheValueAsync<T>(string key, T value)
    {
        var db = _redisConnection.GetDatabase();
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serializedValue);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Serialization error: {ex.Message}");
        }
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        var db = _redisConnection.GetDatabase();
        var cachedValue = await db.StringGetAsync(key);

        if (cachedValue.IsNullOrEmpty)
        {
            Console.WriteLine($"No data found for key: {key}");
            return default;
        }

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Console.WriteLine($"[INFO]: Value for key '{key}' successfully retrieved from Redis cache.");
            return JsonSerializer.Deserialize<T>(cachedValue, options);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Deserialize error for key '{key}': {ex.Message}");
            Console.WriteLine($"Cached value: {cachedValue}");
            return default;
        }
    }
    
    public async Task SetCacheValueAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    public async Task<string> GetCacheValueAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        var cachedValue = await db.StringGetAsync(key);

        if (cachedValue.IsNullOrEmpty)
            return null;

        return cachedValue.ToString();
    }
    
    public async Task DeleteCacheValueAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
