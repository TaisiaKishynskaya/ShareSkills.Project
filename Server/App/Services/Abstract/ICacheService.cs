namespace App.Services.Abstract;

public interface ICacheService
{
    Task SetCacheValueAsync<T>(string key, T value);
    Task<T> GetCacheValueAsync<T>(string key);
    
    Task SetCacheValueAsync(string key, string value); 
    Task<string> GetCacheValueAsync(string key); 

    Task DeleteCacheValueAsync(string key); 
}
