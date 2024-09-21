using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;
using System.Net.Http;
using RichardSzalay.MockHttp;
using Microsoft.AspNetCore.Components.Web;

namespace MobileClient.Tests.Pages.Tests
{
    public class ChoosedLessonPageTests : TestContext
    {
        private Mock<ICalendarService> mockCalendarService;
        private Mock<IPreferencesService> mockPreferencesService;
        protected readonly MockHttpMessageHandler mockHttpMessageHandler;
        HttpClient httpClient;


        public ChoosedLessonPageTests()
        {
            mockCalendarService = new Mock<ICalendarService>();
            mockPreferencesService = new Mock<IPreferencesService>();
            mockHttpMessageHandler = new MockHttpMessageHandler();
            httpClient = new HttpClient(mockHttpMessageHandler);
            Services.AddSingleton<ICalendarService>(mockCalendarService.Object);
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
            Services.AddSingleton<HttpClient>(httpClient);
        }

        [Fact]
        public async Task Should_Fetch_Meeting_And_Participant_On_Init()
        {
            // Arrange
            var meeting = new Meeting { Name = "Math Lesson", DateTime = DateTime.Now };
            var participant = new User { Name = "John", Surname = "Doe" };

            mockCalendarService
                .Setup(service => service.GetMeetingInfo(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((meeting, participant));

            mockPreferencesService
                .Setup(service => service.Get("userRole", string.Empty))
                .Returns("some-role");

            var cut = RenderComponent<ChoosedLesson>(parameters => parameters.Add(p => p.Id, "123"));

            // Assert
            var meetingName = cut.Find("p").TextContent;
            var participantName = cut.FindAll("p.text-value")[0].TextContent;

            Assert.Contains("Math Lesson", meetingName);
            Assert.Contains("John Doe", participantName);
        }

        [Fact]
        public async Task Should_Cancel_Meeting_And_Navigate_Back_On_Success()
        {
            // Arrange

            var meeting = new Meeting { Name = "Math Lesson", DateTime = DateTime.Now };
            var participant = new User { Name = "John", Surname = "Doe", Role="1234" };
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            mockCalendarService
                .Setup(service => service.GetMeetingInfo(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((meeting, participant));

            mockHttpMessageHandler.When("http://localhost:5115/meetings/123").Respond(req => new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK });

            mockPreferencesService
                .Setup(service => service.Get("userRole", string.Empty))
                .Returns("1234");

            var cut = RenderComponent<ChoosedLesson>(parameters => parameters.Add(p => p.Id, "123"));

            // Act
            var cancelButton = cut.Find("button.cancel");
            await cancelButton.ClickAsync(new MouseEventArgs());

            // Assert
            //var navigationHistory = navMan.History.Single();
            Assert.Equal("http://localhost/calendar", navMan.Uri);
        }

        [Fact]
        public void Should_Navigate_Back_To_Calendar_On_Back_Button_Click()
        {
            // Arrange
            var meeting = new Meeting { Name = "Math Lesson", DateTime = DateTime.Now };
            var participant = new User { Name = "John", Surname = "Doe" };
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            mockCalendarService
                .Setup(service => service.GetMeetingInfo(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((meeting, participant));

            mockPreferencesService
                .Setup(service => service.Get("userRole", string.Empty))
                .Returns("some-role");

            var cut = RenderComponent<ChoosedLesson>(parameters => parameters.Add(p => p.Id, "123"));

            // Act
            var backButton = cut.Find("svg");
            backButton.Click();

            // Assert
            Assert.Equal("http://localhost/calendar", navMan.Uri);
        }
    }
}
