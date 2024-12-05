using MobileClient.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Newtonsoft.Json;

namespace MobileClient.Tests.Service.Tests
{
    public class ReportsServiceTests : TestsService
    {
        private readonly ReportsService reportsService;

        public ReportsServiceTests()
        {
            mockPreferencesService.Setup(p => p.Get("userId", It.IsAny<string>())).Returns("mocked_userId");
            reportsService = new ReportsService(httpClient, mockPreferencesService.Object);
        }

        [Fact]
        public async Task getReports_ShouldReturnReportsList_WhenSuccessful()
        {
            var expected_list = new List<Report> { new Report { teacherName = "teacher name", teacherSurname = "teacher surname", studentName = "student name", studentSurname = "student surname", skillName = "skill name", themes = new List<String> { "theme1", "theme2" }, totalTime = 10 } };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expected_list)
            };

            mockHttpMessageHandler.When($"{fakeBaseAddres}/reports/learning/mocked_userId").Respond(req => expectedResponse);

            var response = await reportsService.getReports();

            Assert.Equal(JsonConvert.SerializeObject(expected_list), JsonConvert.SerializeObject(response));
        }

        [Fact]
        public async Task getReports_ShouldReturnEmptyList_WhenUnsuccessful()
        {
            var expected_list = new List<Report> {};
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            };

            mockHttpMessageHandler.When($"{fakeBaseAddres}/reports/learning/mocked_userId").Respond(req => expectedResponse);

            var response = await reportsService.getReports();

            Assert.Equal(JsonConvert.SerializeObject(expected_list), JsonConvert.SerializeObject(response));
        }
    }
}
