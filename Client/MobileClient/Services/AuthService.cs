using System.Net.Http.Json;

namespace MobileClient.Services;

public class AuthService
{

    private readonly HttpClient _httpClient;
    private AuthResponse authResponse; 
    private User user;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> UserLogin(string email, string password)
    {
        
        try {
            Console.WriteLine($"http://localhost:5115/login?email={email}&password={password}");
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5115/login?email={email}&password={password}", new {});
            Console.Write(response);
            if (response.IsSuccessStatusCode)
            {
                authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                Preferences.Set("userId", authResponse.userId);
                // Preferences for saving data
                Preferences.Set("jwt", authResponse.token);
                Console.WriteLine("jwt: "+Preferences.Get("jwt", string.Empty));
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> Register(bool IsTeacher, string Name, string Surname, String Email, string Password)
    {
        var Role = IsTeacher ? "teacher" : "student";
        var requestData = new
        {
            Name,
            Surname,
            Email,
            Password,
            Role
        };
        try {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/register", requestData);
            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadFromJsonAsync<string?>();
                Preferences.Set("userId", userId);
                Console.WriteLine(userId);
                await UserLogin(Email, Password);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task getUserRole()
    {
        var userId = Preferences.Get("userId", string.Empty);
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadFromJsonAsync<User>();
                Preferences.Set("userRole", user.Role);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task<List<Skill>?> GetSkills()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Preferences.Get("jwt", string.Empty));
            var response = await _httpClient.GetAsync($"http://localhost:5115/skills");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Skill>>();
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<bool> ChangeSkills(string id, string skill, string time, string level)
    {
        var requestData = new
        {
            userId=id,
            rating=0,
            classTime=time,
            level=level,
            skill=skill
        };
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/teachers", requestData);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("teacher info updated");
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


public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
}

public class AuthResponse
{
    public string token { get; set; }
    public string userId { get; set; }
}

public class Skill
{
    public string id {get; set;}
    public string skill {get; set;}
}