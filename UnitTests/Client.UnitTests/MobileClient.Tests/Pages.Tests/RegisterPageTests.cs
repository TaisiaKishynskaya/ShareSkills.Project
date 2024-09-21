using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;


namespace MobileClient.Tests.Pages.Tests
{
    public class RegisterPageTests : TestContext
    {
        private Mock<IAuthService> mockAuthService;

        public RegisterPageTests() 
        {
            mockAuthService = new Mock<IAuthService>();
            Services.AddSingleton<IAuthService>(mockAuthService.Object);
        }


        [Fact]
        public void ShouldRenderRegistrationForm_WithCorrectElements()
        {
            // Arrange
            var component = RenderComponent<Registration>();

            // Assert
            component.Find("input[placeholder='Name']").MarkupMatches("<input type=\"text\" class=\"main-input\" placeholder=\"Name\">");
            component.Find("input[placeholder='Surname']").MarkupMatches("<input type=\"text\" class=\"main-input\" placeholder=\"Surname\">");
            component.Find("input[type='email']").MarkupMatches("<input type=\"email\" class=\"main-input\" placeholder=\"email\">");
            component.Find("input[placeholder='Password']").MarkupMatches("<input type=\"password\" class=\"main-input\" placeholder=\"Password\">");
            component.Find("input[type='checkbox']").MarkupMatches("<input type=\"checkbox\">");
            component.Find("button").MarkupMatches("<button class=\"next-btn\">Next</button>");
        }

        [Fact]
        public async Task ShouldShowErrorMessage_WhenRegistrationFails()
        {
            // Arrange
            mockAuthService.Setup(x => x.Register(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);
            var component = RenderComponent<Registration>();

            // Act
            component.Find("input[placeholder='Name']").Change("John");
            component.Find("input[placeholder='Surname']").Change("Doe");
            component.Find("input[type='email']").Change("john.doe@example.com");
            component.Find("input[placeholder='Password']").Change("password123");
            await component.Find("button").ClickAsync(null);

            // Assert
            component.Find("p[class='error']").MarkupMatches("<p class=\"error\">An error occurred</p>");
        }

        [Fact]
        public async Task ShouldNavigateToChooseSkills_WhenRegisterAsTeacherSuccessful()
        {
            // Arrange
            mockAuthService.Setup(x => x.Register(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            var component = RenderComponent<Registration>();

            // Act
            component.Find("input[placeholder=Name]").Change("John");
            component.Find("input[placeholder=Surname]").Change("Doe");
            component.Find("input[placeholder=email]").Change("john.doe@example.com");
            component.Find("input[placeholder=Password]").Change("password123");
            component.Find("input[type=checkbox]").Change(true); // Teacher
            component.Find("button").Click();

            // Assert
            var navigationHistory = navMan.History.Single();
            mockAuthService.Verify(x => x.Register(true, "John", "Doe", "john.doe@example.com", "password123"), Times.Once);
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }

        [Fact]
        public async Task ShouldNavigateToCalendar_WhenRegisterAsStudentSuccessful()
        {
            // Arrange
            mockAuthService.Setup(x => x.Register(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            var component = RenderComponent<Registration>();

            // Act
            component.Find("input[placeholder=Name]").Change("John");
            component.Find("input[placeholder=Surname]").Change("Doe");
            component.Find("input[placeholder=email]").Change("john.doe@example.com");
            component.Find("input[placeholder=Password]").Change("password123");
            component.Find("input[type=checkbox]").Change(false); // Student
            component.Find("button").Click();

            // Assert
            var navigationHistory = navMan.History.Single();
            mockAuthService.Verify(x => x.Register(false, "John", "Doe", "john.doe@example.com", "password123"), Times.Once);
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }

        [Fact]
        public async Task ShouldNavigateToLogin_WhenLoginLinkIsClicked()
        {
            // Arrange
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            var component = RenderComponent<Registration>();

            // Act
            component.Find("a").Click();

            // Assert
            var navigationHistory = navMan.History.Single();
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }

    }
}
