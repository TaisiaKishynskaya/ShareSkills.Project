using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;

namespace MobileClient.Tests.Pages.Tests
{
    public class ChoosedMentorPageTests : TestContext
    {
        private Mock<ISearchService> mockSearchService;
        private Mock<IPreferencesService> mockPreferencesService;

        public ChoosedMentorPageTests()
        {
            mockSearchService = new Mock<ISearchService>();
            mockPreferencesService = new Mock<IPreferencesService>();
            Services.AddSingleton<ISearchService>(mockSearchService.Object);
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
        }

        [Fact]
        public async Task Should_Display_Teacher_Info_When_Found()
        {
            // Arrange
            var teacher = new Teacher
            {
                name = "John",
                surname = "Doe",
                rating = 4.5,
                email = "john.doe@example.com",
                skill = "C#"
            };

            mockSearchService
                .Setup(service => service.GetTeacherById(It.IsAny<string>()))
                .ReturnsAsync(teacher);

            var cut = RenderComponent<ChoosedMentor>(parameters => parameters.Add(p => p.Id, "123"));

            // Assert
            Assert.Contains("John Doe", cut.Markup);  // Check if the teacher's name is displayed
            Assert.Contains("john.doe@example.com", cut.Markup);  // Check if the email is displayed
            Assert.Contains("C#", cut.Markup);  // Check if the skill is displayed
            Console.WriteLine(cut.Markup);
            Assert.Contains("4,5", cut.Markup);  // Check if the rating is displayed
        }

        [Fact]
        public void Should_Navigate_Back_When_Back_Button_Clicked()
        {
            // Arrange
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            var teacher = new Teacher
            {
                name = "John",
                surname = "Doe",
                rating = 4.5,
                email = "john.doe@example.com",
                skill = "C#"
            };

            mockSearchService
                .Setup(service => service.GetTeacherById(It.IsAny<string>()))
                .ReturnsAsync(teacher);

            var cut = RenderComponent<ChoosedMentor>(parameters => parameters.Add(p => p.Id, "123"));

            // Act
            var backButton = cut.Find("svg");  // Assuming the back button is an SVG element
            backButton.Click();  // Simulate back button click

            // Assert
            Assert.Equal("http://localhost/search", navMan.Uri);  // Check if navigation was to the search page
        }

        [Fact]
        public async Task Should_Handle_Null_Teacher_Info_Gracefully()
        {
            // Arrange
            mockSearchService
                .Setup(service => service.GetTeacherById(It.IsAny<string>()))
                .ReturnsAsync((Teacher)null);  // Simulate a missing teacher

            var cut = RenderComponent<ChoosedMentor>(parameters => parameters.Add(p => p.Id, "123"));

            // Assert
            Assert.DoesNotContain("John Doe", cut.Markup);  // Ensure that no teacher info is displayed
            Assert.DoesNotContain("loading...", cut.Markup);  // Ensure that loading state is no longer displayed
        }
    }
}
