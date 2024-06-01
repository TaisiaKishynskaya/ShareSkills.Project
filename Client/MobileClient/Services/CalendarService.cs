using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

    public async Task<bool> AddMeeting(DateTime Date, string NameToCreate)
    {
        var postData = new
        {
            dateAndTime = Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ownerId = Preferences.Get("userId", string.Empty),
            foreignId = NameToCreate
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/meetings", postData);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Meeting created");
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

public class Meeting
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Description { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ForeignId { get; set; }
}