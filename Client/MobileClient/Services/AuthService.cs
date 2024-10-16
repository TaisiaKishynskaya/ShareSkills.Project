using MobileClient.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace MobileClient.Services;

public class AuthService : IAuthService
{

    private readonly HttpClient _httpClient;
    private readonly IPreferencesService _preferencesService;
    private AuthResponse authResponse; 
    private User user;

    public AuthService(HttpClient httpClient, IPreferencesService preferences)
    {
        _httpClient = httpClient;
        _preferencesService = preferences;
    }

    public async Task<bool> UserLogin(string email, string password)
    {
        await GetCookies();
        try {
            System.Diagnostics.Debug.Print("headers from login method:");
            foreach (var header in _httpClient.DefaultRequestHeaders)
            {
                System.Diagnostics.Debug.Print($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            System.Diagnostics.Debug.Print("isCookie: "+_httpClient.DefaultRequestHeaders.Contains("Cookie").ToString());
            HttpResponseMessage ? response = null;
            if (!_httpClient.DefaultRequestHeaders.Contains("Cookie"))
            {
                System.Diagnostics.Debug.Print("login with jwt");
                response = await _httpClient.PostAsJsonAsync($"http://localhost:5115/login?email={email}&password={password}&authMethodCookie=false", new { });
            }
            else
            {
                System.Diagnostics.Debug.Print("login with cookie");
                response = await _httpClient.PostAsJsonAsync($"http://localhost:5115/login?email={email}&password={password}&authMethodCookie=true", new { });
            }
            System.Diagnostics.Debug.Print("response headers: "+response.Headers);
            if (response.IsSuccessStatusCode)
            {
                authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                System.Diagnostics.Debug.Print("userid: "+authResponse.userId);
                _preferencesService.Set("userId", authResponse.userId);

                // Preferences for saving data
                Console.WriteLine("jwt: " + _preferencesService.Get("jwt", string.Empty));

                if (_preferencesService.Get("allowCookies", String.Empty)=="true" && response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
                {
                    string cookie = cookieHeaders.FirstOrDefault();
                    if (cookie != null)
                    {
                        // ��������� ����
                        _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                        _preferencesService.Set("cookie", cookie);
                        System.Diagnostics.Debug.Print("Saved Cookie: " + cookie);
                    }
                }
                else
                {
                    _preferencesService.Set("jwt", authResponse.token);
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _preferencesService.Get("jwt", string.Empty));
                }

                await getUserRole();
                return true;
            }
            else
            {
                System.Diagnostics.Debug.Print("status code " + response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print("Error occurred: " + ex.Message);
            System.Diagnostics.Debug.Print("trace: " + ex.StackTrace);
            return false;
        }
    }

    public async Task<ValidationResponse> Register(bool IsTeacher, string Name, string Surname, String Email, string Password)
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
                _preferencesService.Set("userId", userId);
                Console.WriteLine(userId);
                await UserLogin(Email, Password);
                return new ValidationResponse() { Succesful=true, Errors=null};
            }
            else
            {
                System.Diagnostics.Debug.Print("status: "+response.StatusCode.ToString());
                var errors = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
                return new ValidationResponse() { Succesful = false, Errors = errors.Errors };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print("error: " + ex.StackTrace);
            return new ValidationResponse() { Succesful = false, Errors = null };
        }
    }

    public async Task getUserRole()
    {
        var userId = _preferencesService.Get("userId", string.Empty);
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadFromJsonAsync<User>();
                _preferencesService.Set("userRole", user.Role);
                System.Diagnostics.Debug.Print("role was set " + user.Role);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }

    public async Task<List<Skill>?> GetSkills()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _preferencesService.Get("jwt", string.Empty));
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
            System.Diagnostics.Debug.Print(ex.StackTrace);
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
            System.Diagnostics.Debug.Print(ex.StackTrace);
            return false;
        }
    }

    public async Task AllowCookies()
    {
        _preferencesService.Set("allowCookies", "true");

    }

    public async Task DenyCookies()
    {
        _preferencesService.Set("allowCookies", "false");

    }

    public async Task<string> GetCookiesPermission()
    {
        return _preferencesService.Get("allowCookies", string.Empty);
    }

    public async Task<bool> GetCookies()
    {
        var savedCookie = _preferencesService.Get("cookie", string.Empty);
        if (!string.IsNullOrEmpty(savedCookie))
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie", savedCookie);
            System.Diagnostics.Debug.Print("Cookies were set");
            return true;
        }
        return false;
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

public class ValidationResponse
{
    public bool Succesful { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; }
}

public class ValidationErrorResponse
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
}