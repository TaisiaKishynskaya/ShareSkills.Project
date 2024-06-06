using System.Net.Http.Json;

public class SearchService
{
    private readonly HttpClient _httpClient;

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Teacher?> SearchTeacher(string skill, string time, string level)
    {
        try
        {
            var encodedSkill = Uri.EscapeDataString(skill);
            var encodedLevel = Uri.EscapeDataString(level);
            var encodedTime = Uri.EscapeDataString(time);
            var response = await _httpClient.GetAsync($"http://localhost:5115/get-teacher?skill={encodedSkill}&level={encodedLevel}&classTime={encodedTime}");
            if (response.IsSuccessStatusCode)
            {
                var teacherId = await response.Content.ReadFromJsonAsync<string>();
                var Resteacher = await GetTeacherById(teacherId);
                return Resteacher;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<Teacher?> GetTeacherById(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/teachers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var teacher = await response.Content.ReadFromJsonAsync<Teacher>();
                return teacher;
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

    public async Task<Teacher?> GetTeacherByEmail(string email)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5115/teachers/get-by-email/{email}");
            if (response.IsSuccessStatusCode)
            {
                var teacherId = await response.Content.ReadFromJsonAsync<string>();
                var teacher = await GetTeacherById(teacherId);
                return teacher;
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
    public string name { get; set; }
    public string surname { get; set; }
    public string email { get; set; }
    public string id { get; set; }
    public double rating { get; set; }
    public string classTime { get; set; }
    public string level { get; set; }
    public string skill { get; set; }
}