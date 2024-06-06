using System.Net.Http.Json;

public class SearchService
{
    private readonly HttpClient _httpClient;

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> SearchTeacher(string skill, string time, string level)
    {
        try
        {
            var encodedSkill = Uri.EscapeDataString(skill);
            var encodedLevel = Uri.EscapeDataString(level);
            var encodedTime = Uri.EscapeDataString(time);
            var response = await _httpClient.GetAsync($"http://localhost:5115/get-teacher?skillId={encodedSkill}&levelId={encodedTime}&classTimeId={encodedLevel}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<string>();
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