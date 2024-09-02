using System.Net.Http.Json;

namespace MobileClient.Services;

public class CabinetService
{
    private readonly HttpClient _httpClient;

    public CabinetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> GetUser()
    {
        var userId = Preferences.Get("userId", string.Empty);
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
        var userId = Preferences.Get("userId", string.Empty);
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
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Preferences.Get("jwt", string.Empty));
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