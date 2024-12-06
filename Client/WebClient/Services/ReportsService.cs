using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace WebClient.Services;

public class ReportsService : IReportsService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ReportsService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Report>> getReports()
    {
        try
        {
            var userId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
            HttpResponseMessage response =
                await _httpClient.GetAsync($"http://localhost:5115/reports/learning/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Report>>();
            }

            return new List<Report>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print("reports error: " + ex.Message + ex.StackTrace);
            return new List<Report>();
        }
    }
}

public class Report
{
    public String teacherName { get; set; }
    public String teacherSurname { get; set; }
    public String studentName { get; set; }
    public String studentSurname { get; set; }
    public String skillName { get; set; }
    public int totalTime { get; set; }
    public List<String> themes { get; set; }
}
