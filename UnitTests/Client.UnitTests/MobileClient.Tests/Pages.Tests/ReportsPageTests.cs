using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MobileClient.Components.Pages;
using MobileClient.Services;
using Moq;
using Xunit;

namespace MobileClient.Tests.Pages.Tests;
public class ReportsPageTests : TestContext
{
    [Fact]
    public void DisplaysTitleAndButtons_WhenReportsAreNotEmpty()
    {
        // Arrange
        var mockService = new Mock<IReportsService>();
        mockService.Setup(service => service.getReports())
                   .ReturnsAsync(new List<Report> { new Report { teacherName = "teacher name", teacherSurname = "teacher surname", studentName = "student name", studentSurname = "student surname", skillName = "skill name", themes = new List<String> { "theme1", "theme2" }, totalTime = 10 } });

        Services.AddSingleton(mockService.Object);

        // Act
        var component = RenderComponent<Reports>();

        // Assert
        Assert.Contains("Available reports", component.Markup);
        Assert.Contains("skill name", component.Markup);
    }

    [Fact]
    public void DisplaysNoReportsMessage_WhenReportsAreEmpty()
    {
        // Arrange
        var mockService = new Mock<IReportsService>();
        mockService.Setup(service => service.getReports()).ReturnsAsync(new List<Report>());
        Services.AddSingleton(mockService.Object);

        // Act
        var component = RenderComponent<Reports>();

        // Assert
        Assert.Contains("No available reports yet", component.Markup);
    }

    [Fact]
    public void ShowReportDialog_WhenButtonClicked()
    {
        // Arrange
        var mockService = new Mock<IReportsService>();
        mockService.Setup(service => service.getReports())
                   .ReturnsAsync(new List<Report>
                   {
                       new Report
                       {
                           skillName = "Math",
                           themes = new List<string> { "Algebra", "Geometry" },
                           studentName = "John",
                           studentSurname = "Doe",
                           teacherName = "Jane",
                           teacherSurname = "Smith",
                           totalTime = 10
                       }
                   });

        Services.AddSingleton(mockService.Object);

        // Act
        var component = RenderComponent<Reports>();
        var button = component.Find("button");
        button.Click();

        // Assert
        Assert.Contains("John Doe", component.Markup);
        Assert.Contains("report from: Jane Smith", component.Markup);
        Assert.Contains("subject: Math", component.Markup);
        Assert.Contains("Algebra, Geometry", component.Markup);
    }

    [Fact]
    public void NavigateToCabinet_WhenBackButtonClicked()
    {
        // Arrange
        var mockService = new Mock<IReportsService>();
        mockService.Setup(service => service.getReports()).ReturnsAsync(new List<Report>());
        Services.AddSingleton(mockService.Object);
        var navMan = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var component = RenderComponent<Reports>();
        var svg = component.Find("svg");
        svg.Click();

        // Assert
        var navigationHistory = navMan.History.Single();
        Assert.Equal("/cabinet", navigationHistory.Uri);
    }
}
