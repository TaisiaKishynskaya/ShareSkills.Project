using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Bunit.TestDoubles;

namespace MobileClient.Tests.Pages.Tests
{
    public class LoginPageTests : TestContext
    {
        [Fact]
        public async Task ShouldRenderLoginForm_WithCorrectElements()
        {
            // Act
            var mockAuthService = new Mock<IAuthService>();
            Services.AddSingleton(mockAuthService.Object);
            var component = RenderComponent<Login>();

            // Assert - Check that the email and password inputs are rendered
            component.Find("input[type='email']").MarkupMatches("<input type=\"email\" class=\"main-input\" placeholder=\"Email\">");
            component.Find("input[type='password']").MarkupMatches("<input type=\"password\" class=\"main-input\" placeholder=\"Password\">");
            component.Find("button.login-btn").MarkupMatches("<button class=\"login-btn\">Login</button>");
        }

        [Fact]
        public async Task ShouldShowErrorMessage_WhenLoginFails()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.UserLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            Services.AddSingleton(mockAuthService.Object); // Register mock AuthService

            // Act
            var component = RenderComponent<MobileClient.Components.Pages.Login>();

            component.Find("input[type='email']").Change("wrong@example.com");
            component.Find("input[type='password']").Change("wrongpassword");

            // Trigger login button
            component.Find("button.login-btn").Click();

            // Assert
            Assert.Contains("Incorrect email or password", component.Markup);
        }

        [Fact]
        public async Task ShouldNavigateToCalendar_WhenLoginSucceeds()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.UserLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            Services.AddSingleton(mockAuthService.Object); // Register mock AuthService
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            // Act
            var component = RenderComponent<Login>();

            component.Find("input[type='email']").Change("test@example.com");
            component.Find("input[type='password']").Change("password");

            // Trigger login button
            component.Find("button.login-btn").Click();

            // Assert
            var navigationHistory = navMan.History.Single();
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);
        }
    }
}
