using Moq;
using System.Net.Http.Json;
using System.Net;
using RichardSzalay.MockHttp;
using MobileClient.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace MobileClient.Tests.Service.Tests
{
    public class FeedbackServiceTests : TestsService
    {
        private readonly FeedbackService _feedbackService;
        private readonly Mock<ISearchService> _mockSearchService;
        private readonly Mock<ILogger<FeedbackService>> _mockLogger;

        public FeedbackServiceTests()
        {
            _mockSearchService = new Mock<ISearchService>();
            _mockLogger = new Mock<ILogger<FeedbackService>>();
            _feedbackService = new FeedbackService(httpClient, _mockSearchService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendFeedback_ShouldReturnTrue_IfSuccesful()
        {
            var mockTeacher = new Teacher
            {
                id = "1",
                rating = 5
            };
            _mockSearchService.Setup(s => s.GetTeacherByEmail(It.IsAny<string>())).ReturnsAsync(mockTeacher);

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
            };
            mockHttpMessageHandler.When($"{fakeBaseAddres}/grades").Respond(req => expectedResponse);

            var response = await _feedbackService.SendFeedback("email", 5);

            Assert.True(response);
        }

        [Fact]
        public async Task SendFeedback_ShouldReturnFalse_WhenTeacherIsNull()
        {
            Teacher? t = null;
            _mockSearchService.Setup(s => s.GetTeacherByEmail(It.IsAny<string>())).ReturnsAsync(t);

            var response = await _feedbackService.SendFeedback("email", 5);

            Assert.False(response);
        }

        [Fact]
        public async Task SwndFeedback_ShouldReturnFalse_IfEmailIsEmpty()
        {
            var response = await _feedbackService.SendFeedback("", 5);

            Assert.False(response);
        }
    }
}
