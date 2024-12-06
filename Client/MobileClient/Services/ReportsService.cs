using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public class ReportsService : IReportsService
    {

        private readonly HttpClient _httpClient;
        private readonly IPreferencesService _preferencesService;

        public ReportsService(HttpClient httpClient, IPreferencesService preferencesService)
        {
            _httpClient = httpClient;
            _preferencesService = preferencesService;
        }

        public async Task<List<Report>> getReports()
        {
            try
            {
                var userId = _preferencesService.Get("userId", string.Empty);
                HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5115/reports/learning/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Report>>();
                }
                else
                {
                    return new List<Report> { };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("reports error: " + ex.Message + ex.StackTrace);
                return new List<Report> { };
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
}
