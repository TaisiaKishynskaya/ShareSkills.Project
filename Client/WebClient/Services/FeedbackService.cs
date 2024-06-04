using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System;

public class FeedbackService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public FeedbackService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> SendFeedback(string Id, int Grade)
    {
        var requestBody = new
        {
            teacherId = Id,
            grade = Grade
        };
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/grades", requestBody);
            if (response.IsSuccessStatusCode)
            {
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
