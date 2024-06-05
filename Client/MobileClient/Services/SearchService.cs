using System.Net.Http.Json;

public class SearchService
{
    private readonly HttpClient _httpClient;

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Teacher?> SearchTeacher()
    {
        var skill = Preferences.Get("skill", String.Empty);
        var time = Preferences.Get("time", String.Empty);
        var level = Preferences.Get("level", String.Empty);
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/get-teacher?skillId={skill}&levelId={time}&classTimeId={level}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Teacher>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<List<Teacher>?> GetTeachers()
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/teachers");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Teacher>>();
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

public class Teacher
{
    public string id { get; set; }
    public int rating { get; set; }
    public string classTime { get; set; }
    public string level { get; set; }
    public string skill { get; set; }
}