using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Routing;

namespace MobileClient.Tests.Pages.Tests
{
    public class CalendarPageTests : TestContext
    {
        private Mock<ICalendarService> mockCalendarService;
        private Mock<IPreferencesService> mockPreferencesService;

        public CalendarPageTests()
        {
            mockCalendarService = new Mock<ICalendarService>();
            mockPreferencesService = new Mock<IPreferencesService>();
            Services.AddSingleton<ICalendarService>(mockCalendarService.Object);
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
        }

        [Fact]
        public void ShouldRenderWeekDays_WhenComponentIsRendered()
        {
            // Arrange
            var component = RenderComponent<Calendar>();

            // Assert that the week navigation buttons and day sections are rendered
            Assert.Equal(7, component.FindAll("div.day-info").Count); // 7 days in a week
            Assert.Equal(2, component.FindAll("button").Count); // Next and Previous week buttons
        }

        [Fact]
        public async Task ShouldNavigateToNextWeek_WhenNextWeekButtonIsClicked()
        {
            // Arrange
            var component = RenderComponent<Calendar>();

            // Simulate the initial load of meetings for the current week
            mockCalendarService.Setup(x => x.UpdateCalendar()).ReturnsAsync(new List<Meeting>());

            // Act: Click the Next Week button
            component.Find("button:contains('Next Week')").Click();

            // Assert that the method to load the next week was called
            mockCalendarService.Verify(x => x.UpdateCalendar(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldNavigateToPreviousWeek_WhenPreviousWeekButtonIsClicked()
        {
            // Arrange
            var component = RenderComponent<Calendar>();

            // Simulate the initial load of meetings for the current week
            mockCalendarService.Setup(x => x.UpdateCalendar()).ReturnsAsync(new List<Meeting>());

            // Act: Click the Previous Week button
            component.Find("button:contains('Previous Week')").Click();

            // Assert that the method to load the previous week was called
            mockCalendarService.Verify(x => x.UpdateCalendar(), Times.AtLeastOnce);
        }

        [Fact]
        public void ShouldDisplayAddMeetingDialog_WhenPlusButtonIsClicked()
        {
            // Arrange
            mockPreferencesService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("90c08b8a-fa4c-445e-9f66-717bf2bfcf72"); // Simulate teacher role
            var component = RenderComponent<Calendar>();

            // Act: Click the plus button
            component.Find("div.plus-button").Click();

            // Assert that the dialog is shown
            Assert.Single(component.FindAll("div.dialog"));
        }

        [Fact]
        public async Task ShouldAddMeeting_WhenDialogIsSubmitted()
        {
            // Arrange
            mockPreferencesService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("90c08b8a-fa4c-445e-9f66-717bf2bfcf72"); // Simulate teacher role
            mockCalendarService.Setup(x => x.AddMeeting(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true); // Mock successful meeting creation

            var component = RenderComponent<Calendar>();

            // Act: Click the plus button to show the dialog
            component.Find("div.plus-button").Click();

            // Fill out the form fields
            component.Find("input[placeholder='Email']").Change("test@example.com");
            component.Find("input[placeholder='Title']").Change("New Meeting");
            component.Find("input[type='datetime-local']").Change(DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));

            // Submit the form
            component.Find("button:contains('Create')").Click();

            // Assert that the meeting was added
            mockCalendarService.Verify(x => x.AddMeeting(It.IsAny<DateTime>(), "test@example.com", "New Meeting"), Times.Once);
        }

        [Fact]
        public async Task ShouldNotAddMeeting_WhenFieldsEmpty()
        {
            // Arrange
            mockPreferencesService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("90c08b8a-fa4c-445e-9f66-717bf2bfcf72"); // Simulate teacher role
            mockCalendarService.Setup(x => x.AddMeeting(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true); // Mock successful meeting creation

            var component = RenderComponent<Calendar>();

            // Act: Click the plus button to show the dialog
            component.Find("div.plus-button").Click();

            // Fill out the form fields
            component.Find("input[placeholder='Email']").Change("test@example.com");
            //component.Find("input[placeholder='Title']").Change(null);
            component.Find("input[type='datetime-local']").Change(DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));

            // Submit the form
            component.Find("button:contains('Create')").Click();

            // Assert that the meeting was added
            mockCalendarService.Verify(x => x.AddMeeting(It.IsAny<DateTime>(), "test@example.com", "New Meeting"), Times.Never);
            component.Find("p.error").MarkupMatches("<p class=\"error\">all fields must be filled in</p>");
        }

        [Fact]
        public void ShouldNavigateToLessonDetails_WhenLessonIsClicked()
        {
            // Arrange
            var meetingId = Guid.NewGuid();
            var ForeignId = Guid.NewGuid();
            var OwnerId = Guid.NewGuid();
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            mockPreferencesService.Setup(x => x.Get("userId", string.Empty)).Returns(ForeignId.ToString());
            mockCalendarService.Setup(x => x.UpdateCalendar()).ReturnsAsync(new List<Meeting>
            {
                new Meeting { Id = meetingId, Name = "Test Lesson", DateTime = DateTime.Now, Description="desc", ForeignId=ForeignId, OwnerId=OwnerId }
            });
            var component = RenderComponent<Calendar>();

            // Act: Click on a lesson card
            component.Find("div.lesson-card").Click();

            // Assert that navigation to the lesson details page occurred
            var navigationHistory = navMan.History.Single();
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }
    }
}
