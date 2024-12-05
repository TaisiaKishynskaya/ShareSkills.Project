using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;
namespace MobileClient.Tests.Pages.Tests
{
    public class CabinetPageTests : TestContext
    {
        private Mock<ICabinetService> mockCabinetService;
        private Mock<IPreferencesService> mockPreferencesService;

        public CabinetPageTests()
        {
            mockCabinetService = new Mock<ICabinetService>();
            mockPreferencesService = new Mock<IPreferencesService>();
            Services.AddSingleton<ICabinetService>(mockCabinetService.Object);
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
        }

        [Fact]
        public async Task OnInitializedAsync_ShouldLoadUserData()
        {
            // Arrange
            var expectedUser = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                PasswordHash = "passwordhash"
            };
            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(expectedUser);

            var component = RenderComponent<Cabinet>();

            // Assert
            component.Find("p.name").MarkupMatches("<p class=\"name\">John Doe</p>");
            component.Find("p.email").MarkupMatches("<p class=\"email\">john@example.com</p>");
        }

        [Fact]
        public void Should_Show_Dialog_On_ChangeProfile_Button_Click()
        {
            // Arrange
            var expectedUser = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                PasswordHash = "passwordhash"
            };
            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(expectedUser);

            var cut = RenderComponent<Cabinet>();

            // Act
            var button = cut.Find("button.feedback");
            button.Click();

            // Assert
            var dialog = cut.Find("div.dialog");
            Assert.NotNull(dialog);
        }

        [Fact]
        public async Task Should_Change_Profile_Info()
        {
            // Arrange
            var user = new User { Name = "John", Surname = "Doe", Email = "john.doe@example.com" };

            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(user);
            mockCabinetService.Setup(service => service.ChangeInfo(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            var cut = RenderComponent<Cabinet>();
            cut.Find("button.feedback").Click();

            // Act - Simulate user input
            cut.Find("input[placeholder='Name']").Change("Jane");
            cut.Find("input[placeholder='Surname']").Change("White");
            cut.Find("input[placeholder='Email']").Change("Jane@gmail.com");
            cut.Find("input[placeholder='Password']").Change("Jane123");

            var button = cut.Find("button.next-btn");
            button.Click();

            // Assert
            mockCabinetService.Verify(service => service.ChangeInfo(It.Is<User>(u => u.Name == "Jane"), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ShouldNot_Change_Profile_Info_IfFieldsUnfilled()
        {
            // Arrange
            var user = new User { Name = "John", Surname = "Doe", Email = "john.doe@example.com" };

            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(user);
            mockCabinetService.Setup(service => service.ChangeInfo(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            var cut = RenderComponent<Cabinet>();
            cut.Find("button.feedback").Click();

            // Act - Simulate user input
            cut.Find("input[placeholder='Name']").Change("Jane");
            cut.Find("input[placeholder='Surname']").Change("");
            cut.Find("input[placeholder='Email']").Change("Jane@gmail.com");
            cut.Find("input[placeholder='Password']").Change("");

            var button = cut.Find("button.next-btn");
            button.Click();

            // Assert
            mockCabinetService.Verify(service => service.ChangeInfo(It.Is<User>(u => u.Name == "Jane"), It.IsAny<string>()), Times.Never);
            cut.Find("p.error").MarkupMatches("<p class=\"error\">all field must be filed in</p>");
        }

        [Fact]
        public void Should_Navigate_To_FeedbackPage_On_LeaveFeedback_Click()
        {
            // Arrange
            var expectedUser = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                PasswordHash = "passwordhash"
            };
            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(expectedUser);
            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            mockPreferencesService.Setup(p => p.Get("userRole", string.Empty)).Returns("non-teacher-role");

            var cut = RenderComponent<Cabinet>();

            // Act
            cut.FindAll("button.feedback").FirstOrDefault(btn => btn.TextContent.Contains("Leave feedback")).Click();

            // Assert
            var navigationHistory = navMan.History.Single();
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }

        [Theory]
        [InlineData("90c08b8a-fa4c-445e-9f66-717bf2bfcf72", true)]  // Teacher
        [InlineData("some-other-role", false)]                    // Non-Teacher
        public async Task Should_Display_Teacher_NonTeacher_Options(string role, bool isTeacher)
        {
            // Arrange
            var expectedUser = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                PasswordHash = "passwordhash"
            };
            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(expectedUser);
            mockPreferencesService.Setup(p => p.Get("userRole", string.Empty)).Returns(role);

            var cut = RenderComponent<Cabinet>();

            // Assert
            if (isTeacher)
            {
                Assert.NotNull(cut.FindAll("button.feedback").FirstOrDefault(btn => btn.TextContent.Contains("Change skills")));
            }
            else
            {
                Assert.NotNull(cut.FindAll("button.feedback").FirstOrDefault(btn => btn.TextContent.Contains("Leave feedback")));
            }
        }

        [Fact]
        public async Task Should_Change_UserAllowCookies()
        {
            var expectedUser = new User
            {
                Name = "John",
                Surname = "Doe",
                Email = "john@example.com",
                PasswordHash = "passwordhash"
            };
            mockCabinetService.Setup(service => service.GetUser()).ReturnsAsync(expectedUser);
            mockPreferencesService.Setup(s => s.Get("allowCookies", string.Empty)).Returns("false");
            var component = RenderComponent<Cabinet>();
            component.Find(".switch input").Change(true);
            mockPreferencesService.Verify(s => s.Set("allowCookies", "true"));
            component.Find(".switch input").Change(false);
            mockPreferencesService.Verify(s => s.Set("allowCookies", "false"));
        }
    }
}
