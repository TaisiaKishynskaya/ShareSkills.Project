using System.Text;
using System.Text.Json;
using App.Services.Abstract;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private const string ModelName = "llama3.2";

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
    }

    public async Task<string> GetOllamaResponseAsync(string prompt)
    {
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
            return json.RootElement.GetProperty("response").GetString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OLLAMA ERROR] {ex.Message}");
            return $"Error while contacting the model: {ex.Message}";
        }
    }
}