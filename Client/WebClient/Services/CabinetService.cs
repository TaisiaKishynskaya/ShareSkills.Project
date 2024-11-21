using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebClient.Services;

public class CabinetService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public CabinetService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<User?> GetUser()
    {
        var userId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
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

    public async Task<(bool Success, string ErrorMessage)> ChangeInfo(User userToChange, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(userToChange.Name) ||
            string.IsNullOrWhiteSpace(userToChange.Surname) ||
            string.IsNullOrWhiteSpace(userToChange.Email) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            return (false, "All fields must be filled.");
        }

        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(userToChange.Email))
        {
            return (false, "Invalid email format.");
        }

        var userId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
        var requestData = new
        {
            name = userToChange.Name,
            surname = userToChange.Surname,
            email = userToChange.Email,
            password = newPassword
        };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestData),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        try
        {
            var jwt = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await _httpClient.PutAsync($"http://localhost:5115/users/{userId}", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User info changed");
                return (true, string.Empty);
            }

            return (false, $"Server error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return (false, "An error occurred while changing user info.");
        }
    }
}
