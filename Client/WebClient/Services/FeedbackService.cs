using Microsoft.JSInterop;
using System.Net.Http.Json;
namespace WebClient.Services;

public class FeedbackService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly SearchService _searchService;
    private readonly ILogger<FeedbackService> _logger;

    public FeedbackService(HttpClient httpClient, IJSRuntime jsRuntime, SearchService searchService, ILogger<FeedbackService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SendFeedback(string email, int grade)
    {
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogError("Email is null or empty");
            return false;
        }

        try
        {
            var teacher = await _searchService.GetTeacherByEmail(email);
            if (teacher == null)
            {
                _logger.LogError("Teacher not found for email: {Email}", email);
                return false;
            }

            var teacherId = teacher.id;
            _logger.LogInformation("Teacher ID: {TeacherId}", teacherId);

            var requestBody = new
            {
                teacherId = teacherId,
                grade = grade
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/grades", requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response: {ResponseContent}", responseContent);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending feedback");
            return false;
        }
    }
}