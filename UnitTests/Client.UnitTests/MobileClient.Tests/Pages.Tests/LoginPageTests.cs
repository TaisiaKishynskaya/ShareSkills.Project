using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Bunit.TestDoubles;
using System.Net;
using System.Reflection;
using AngleSharp.Dom;

namespace MobileClient.Tests.Pages.Tests
{
    public class LoginPageTests : TestContext
    {
        [Fact]
        public async Task ShouldRenderLoginForm_WithCorrectElements()
        {
            // Act
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var component = RenderComponent<Login>();

            // Assert - Check that the email and password inputs are rendered
            component.Find("input[type='email']").MarkupMatches("<input type=\"email\" class=\"main-input\" placeholder=\"Email\">");
            component.Find("input[type='password']").MarkupMatches("<input type=\"password\" class=\"main-input\" placeholder=\"Password\">");
            component.Find("button.login-btn").MarkupMatches("<button class=\"login-btn\" disabled>Login</button>");
        }

        [Fact]
        public async Task ShouldShowCookieBanner_IfNotChoosen()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.GetCookies()).ReturnsAsync(false);
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var component = RenderComponent<Login>();

            Assert.NotNull(component.Find("div.cookie-banner"));
            Assert.True(component.Find("button.login-btn").IsDisabled());
        }

        [Fact]
        public async Task ShouldNotShowCookieBanner_IfChoosen()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.GetCookies()).ReturnsAsync(true);
            mockAuthService.Setup(s => s.GetCookiesPermission()).ReturnsAsync("true");
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var component = RenderComponent<Login>();

            Assert.Throws<Bunit.ElementNotFoundException>(()=>component.Find("div.cookie-banner"));
        }

        [Fact]
        public async Task ShouldHideCookieBanner_AfterChoise()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("");
            mockAuthService.Setup(s => s.GetCookies()).ReturnsAsync(false);
            mockAuthService.Setup(s => s.GetCookiesPermission()).ReturnsAsync("false");
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var component = RenderComponent<Login>();
            component.Find("button.cookie-btn").Click();
            mockAuthService.Verify(s => s.AllowCookies());
            Assert.Throws<Bunit.ElementNotFoundException>(() => component.Find("div.cookie-banner"));
        }

        [Fact]
        public async Task ShouldAutoLogin_IfCookieAllowed()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.GetCookies()).ReturnsAsync(true);
            mockAuthService.Setup(s => s.GetCookiesPermission()).ReturnsAsync("true");
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            var component = RenderComponent<Login>();

            var navigationHistory = navMan.History.Single();
            Assert.Equal(NavigationState.Succeeded, navigationHistory.State);

        }

        [Fact]
        public async Task ShouldNotAutoLogin_IfCookieDenied()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.GetCookies()).ReturnsAsync(true);
            mockAuthService.Setup(s => s.GetCookiesPermission()).ReturnsAsync("false");
            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(mockPreferences.Object);
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            var component = RenderComponent<Login>();

            var lastEntry = navMan.History.LastOrDefault();
            Assert.Null(lastEntry);

        }

        [Fact]
        public async Task ShouldShowErrorMessage_WhenLoginFails()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.UserLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            Services.AddSingleton(mockAuthService.Object); // Register mock AuthService
            Services.AddSingleton(mockPreferences.Object);

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
            var mockPreferences = new Mock<IPreferencesService>();
            mockPreferences.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("id");
            mockAuthService.Setup(s => s.UserLogin(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            Services.AddSingleton(mockAuthService.Object); // Register mock AuthService
            Services.AddSingleton(mockPreferences.Object);
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
