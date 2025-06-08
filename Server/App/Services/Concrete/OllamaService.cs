using System.Text;
using System.Text.Json;
using App.Services.Abstract;
using App.Infrastructure.Exceptions.Base;
public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private const string ModelName = "llama3.2";
    private readonly ICacheService _cacheService;

    public OllamaService(HttpClient httpClient, ICacheService cacheService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        _cacheService = cacheService;
    }

    public async Task<string> GetOllamaResponseAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new BadRequestException("Prompt cannot be empty.");
        
        var cacheKey = $"ollama:{prompt.Trim()}";
        
        var cachedResponse = await _cacheService.GetCacheValueAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            Console.WriteLine($"[CACHE HIT] Returning cached result for: {prompt}");
            return cachedResponse;
        }
        
        var requestBody = new
        {
            model = ModelName,
            prompt = prompt,
            stream = false
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseString);
            var result = json.RootElement.GetProperty("response").GetString();
            
            if (!string.IsNullOrEmpty(result))
            {
                await _cacheService.SetCacheValueAsync(cacheKey, result);
                Console.WriteLine($"[CACHE SET] Key: {cacheKey}");
            }
            
            return result;

        }
        catch (TaskCanceledException)
        {
            throw new ModelTimeoutException(
                $"Model did not respond within {_httpClient.Timeout.TotalSeconds} seconds");
        }
    }
}