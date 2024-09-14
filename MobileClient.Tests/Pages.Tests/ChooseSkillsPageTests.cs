using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;

namespace MobileClient.Tests.Pages.Tests
{
    public class ChooseSkillsPageTests : TestContext
    {
        private Mock<IPreferencesService> mockPreferencesService;
        private Mock<IAuthService> mockAuthService;

        public ChooseSkillsPageTests() 
        {
            mockPreferencesService = new Mock<IPreferencesService>();
            mockAuthService = new Mock<IAuthService>();
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
            Services.AddSingleton<IAuthService>(mockAuthService.Object);
        }

        [Fact]
        public async Task Should_Display_Skills_List_When_Loaded()
        {
            // Arrange
            var skills = new List<Skill>
            {
                new Skill { skill = "C#" },
                new Skill { skill = "Python" }
            };

            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(skills);

            var cut = RenderComponent<ChooseSkills>();

            // Assert
            Assert.Contains("C#", cut.Markup);
            Assert.Contains("Python", cut.Markup);
        }

        [Fact]
        public async Task Should_Display_Teacher_Fields_When_User_Is_Teacher()
        {
            // Arrange
            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(new List<Skill>());
            mockAuthService.Setup(service => service.getUserRole()).Returns(Task.CompletedTask);
            mockPreferencesService.Setup(p => p.Get("userRole", string.Empty))
                .Returns("90c08b8a-fa4c-445e-9f66-717bf2bfcf72");  // Role that identifies user as a teacher

            var cut = RenderComponent<ChooseSkills>();

            // Assert
            Assert.Contains("Choose a time convenient for learning", cut.Markup);
            Assert.Contains("Morning", cut.Markup);
            Assert.Contains("Day", cut.Markup);
            Assert.Contains("Evening", cut.Markup);
            Assert.Contains("Select the desired level of knowledge", cut.Markup);
            Assert.Contains("Introductory", cut.Markup);
            Assert.Contains("Intermidiate", cut.Markup);
            Assert.Contains("Advanced", cut.Markup);
        }

        [Fact]
        public async Task Should_Navigate_To_Calendar_On_Successful_Skill_Submission()
        {
            // Arrange
            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(new List<Skill>());
            mockAuthService.Setup(service => service.ChangeSkills(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(true);
            mockPreferencesService.Setup(p => p.Get("userId", string.Empty)).Returns("123");

            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            var cut = RenderComponent<ChooseSkills>();

            // Act
            var button = cut.Find("button"); // Find the next button
            button.Click();  // Simulate a click

            // Assert
            Assert.Equal("http://localhost/calendar", navMan.Uri);  // Check if the navigation occurred to the calendar page
        }

        [Fact]
        public async Task Should_Display_Error_On_Failed_Skill_Submission()
        {
            // Arrange
            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(new List<Skill>());
            mockAuthService.Setup(service => service.ChangeSkills(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(false);  // Simulate failure

            mockPreferencesService.Setup(p => p.Get("userId", string.Empty)).Returns("123");

            var cut = RenderComponent<ChooseSkills>();

            // Act
            var button = cut.Find("button"); // Find the next button
            button.Click();  // Simulate a click

            // Assert
            Assert.Contains("An error occured", cut.Markup);  // Check if error message is displayed
        }
    }
}
