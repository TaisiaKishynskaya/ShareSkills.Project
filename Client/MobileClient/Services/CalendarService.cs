using System.Net.Http.Json;

namespace MobileClient.Services;

public class CalendarService
{
    private readonly HttpClient _httpClient;

    public CalendarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Meeting>?> UpdateCalendar()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Preferences.Get("jwt", string.Empty));
            var response = await _httpClient.GetAsync("http://localhost:5115/meetings");
            if (response.IsSuccessStatusCode)
            {
                var meetings = await response.Content.ReadFromJsonAsync<List<Meeting>>();
                if (meetings != null)
                {
                    return meetings;
                }
                return null;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    private async Task<string?> GetIdByEmail(string email)
    {
        Console.WriteLine(email);
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5115/getId?email={email}", new {});
            Console.WriteLine("content: "+await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                var EmailResponse = await response.Content.ReadFromJsonAsync<GetEmailResponse>();
                return EmailResponse.userId;
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
        
    }

    public async Task<bool> AddMeeting(DateTime Date, string Email, String Title)
    {
        var id = await GetIdByEmail(Email);
        var postData = new
        {
            name = Title,
            dateAndTime = Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ownerId = Preferences.Get("userId", string.Empty),
            foreignId = id
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/meetings", postData);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Meeting created");
                return true;
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<(Meeting meeting, User teacher)?> GetMeetingInfo(string Id, String userRole)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/meetings/{Id}");
            if (response.IsSuccessStatusCode)
            {
                var meeting = await response.Content.ReadFromJsonAsync<Meeting>();
                try 
                {
                    var url = (userRole == "90c08b8a-fa4c-445e-9f66-717bf2bfcf72") ? $"http://localhost:5115/users/{meeting.ForeignId}" : $"http://localhost:5115/users/{meeting.OwnerId}";
                    var response2 = await _httpClient.GetAsync(url);
                    if (response2.IsSuccessStatusCode)
                    {
                        var teacher = await response2.Content.ReadFromJsonAsync<User>();
                        return (meeting, teacher);
                    }
                    return null;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
            return null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}

public class Meeting
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime DateTime { get; set; }
    public string Description { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ForeignId { get; set; }
}

public class GetEmailResponse
{
    public string userId { get; set; }
}