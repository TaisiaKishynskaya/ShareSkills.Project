namespace App.Services.Abstract;

public interface IOllamaService
{
    Task<string> GetOllamaResponseAsync(string prompt);
}
