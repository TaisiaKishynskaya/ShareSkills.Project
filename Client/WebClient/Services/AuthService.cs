using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebClient.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private AuthResponse authResponse;
    private User user;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ValidationResponse> UserLogin(string email, string password)
    {
        try
        {
            HttpResponseMessage response;
            string cookieKey = "ShareSkills_App_Cookie";

            // Check user permission for cookies
            var allowCookies = await GetCookiesPermission();

            if (allowCookies == "true")
            {
                // Check if cookie exists in default headers
                var hasCookie = _httpClient.DefaultRequestHeaders.Contains("Cookie");

                // If cookie exists, use it with authMethodCookie=true
                if (hasCookie)
                {
                    string loginUrl = $"http://localhost:5115/login?email={email}&password={password}&authMethodCookie=true";
                    response = await _httpClient.PostAsJsonAsync(loginUrl, new { });
                }
                else
                {
                    // If no cookie, request with authMethodCookie=false and save cookies if available
                    string loginUrl = $"http://localhost:5115/login?email={email}&password={password}&authMethodCookie=false";
                    response = await _httpClient.PostAsJsonAsync(loginUrl, new { });

                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        var cookieHeaders = response.Headers.GetValues("Set-Cookie");
                        var cookie = cookieHeaders.FirstOrDefault();
                        if (cookie != null)
                        {
                            string extractedCookie = ExtractCookieValue(cookie);
                            _httpClient.DefaultRequestHeaders.Add("Cookie", extractedCookie);

                            // Optionally save cookie in local storage or JS runtime
                            await _jsRuntime.InvokeVoidAsync("setCookie", cookieKey, extractedCookie, 30);
                            Console.WriteLine("Saved Cookie: " + extractedCookie);
                        }
                    }
                }
            }
            else
            {
                // User did not allow cookies, always use authMethodCookie=false
                string loginUrl = $"http://localhost:5115/login?email={email}&password={password}&authMethodCookie=false";
                response = await _httpClient.PostAsJsonAsync(loginUrl, new { });
            }

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", authResponse.userId);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", authResponse.token);
                Console.WriteLine("jwt: " + await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt"));

                await GetUserRole();
                return new ValidationResponse { Succesful = true, Errors = null };
            }

            return await HandleErrorResponse(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new ValidationResponse
            {
                Succesful = false,
                Errors = new Dictionary<string, List<string>>
                { { "General", new List<string> { "Unable to connect to the server." } } }
            };
        }
    }

    private string ExtractCookieValue(string setCookieHeader)
    {
        var cookieParts = setCookieHeader.Split(';')[0];
        var keyValue = cookieParts.Split('=');

        if (keyValue.Length == 2)
        {
            return keyValue[1];
        }

        return string.Empty;
    }


    public async Task<ValidationResponse> Register(bool IsTeacher, string Name, string Surname, string Email,
        string Password)
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
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/register", requestData);
            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadFromJsonAsync<string>();
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", userId);
                Console.WriteLine(userId);
                await UserLogin(Email, Password);
                return new ValidationResponse() { Succesful = true, Errors = null };
            }
            else
            {
                var errors = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
                return new ValidationResponse() { Succesful = false, Errors = errors?.Errors };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new ValidationResponse() { Succesful = false, Errors = null };
        }
    }

    private async Task<ValidationResponse> HandleErrorResponse(HttpResponseMessage response)
    {
        var errors = await response.Content.ReadFromJsonAsync<string>();
        return new ValidationResponse
        {
            Succesful = false,
            Errors = new Dictionary<string, List<string>> { { "error", new List<string> { errors } } }
        };
    }

    public async Task GetUserRole()
    {
        var userId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/users/{userId}/role");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                string role = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent)["role"];
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", role);
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
            var jwt = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = await _httpClient.GetAsync("http://localhost:5115/skills");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Skill>>();
            }

            Console.WriteLine($"Error: {response.ReasonPhrase}");
            return null;
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
            userId = id,
            rating = 0,
            classTime = time,
            level = level,
            skill = skill
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

    public async Task AllowCookies()
    {
        await _jsRuntime.InvokeVoidAsync("setCookie", "allowCookies", "true", 30);
        //await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "allowCookies", "true");
        await _jsRuntime.InvokeVoidAsync("setCookie", "ShareSkills_App_Cookie", "cookie_value", 30);
    }

    public async Task DenyCookies()
    {
        await _jsRuntime.InvokeVoidAsync("setCookie", "allowCookies", "false", 30);
        //await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "allowCookies", "false");
    }

    public async Task<string> GetCookiesPermission()
    {
        var allowCookies = await _jsRuntime.InvokeAsync<string>("getCookie", "allowCookies");
        return allowCookies ?? string.Empty;
    }

    public async Task<bool> GetCookies()
    {
        var savedCookie = await _jsRuntime.InvokeAsync<string>("getCookie", "ShareSkills_App_Cookie");
        if (!string.IsNullOrEmpty(savedCookie))
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie", savedCookie);
            Console.WriteLine("Cookies were set");
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
    public string id { get; set; }
    public string skill { get; set; }
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
