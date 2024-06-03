using System.Net.Http.Json;

public class FeedbackService
{
    private readonly HttpClient _httpClient;

    public FeedbackService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendFeedback(String Id, int Grade)
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