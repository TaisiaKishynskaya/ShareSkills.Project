using MobileClient.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class CabinetService
{
    private readonly HttpClient _httpClient;
    private readonly IPreferencesService _preferencesService;

    public CabinetService(HttpClient httpClient, IPreferencesService preferences)
    {
        _httpClient = httpClient;
        _preferencesService = preferences;
    }

    public async Task<User?> GetUser()
    {
        var userId = _preferencesService.Get("userId", string.Empty);
        Console.WriteLine(userId);
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<bool> ChangeInfo(User userToChange, string newPassword) {
        var userId = _preferencesService.Get("userId", string.Empty);
        var requestData = new
        {
            name = userToChange.Name,
            surname = userToChange.Surname,
            email = userToChange.Email,
            password = newPassword
        };
        var jsonContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(requestData),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _preferencesService.Get("jwt", string.Empty));
            var response = await _httpClient.PutAsync($"http://localhost:5115/users/{userId}", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User info changed");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
