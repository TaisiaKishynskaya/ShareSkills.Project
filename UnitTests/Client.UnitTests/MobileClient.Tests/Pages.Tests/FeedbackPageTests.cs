using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;

namespace MobileClient.Tests.Pages.Tests
{
    public class FeedbackPageTests: TestContext
    {
        private Mock<IFeedbackService> mockFeedbackService;
        private Mock<ISearchService> mockSearchService;

        public FeedbackPageTests() 
        {
            mockSearchService = new Mock<ISearchService>();
            mockFeedbackService = new Mock<IFeedbackService>();
            Services.AddSingleton<IFeedbackService>(mockFeedbackService.Object);
            Services.AddSingleton<ISearchService>(mockSearchService.Object);
        }

        [Fact]
        public async Task ShouldLeaveFeedback_IfCorrect()
        {
            mockFeedbackService.Setup(s => s.SendFeedback(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
            var component = RenderComponent<Feedback>();

            component.Find(".text input").Change("email");
            component.Find(".stars-input input").Change(5);
            component.Find("button").Click();

            mockFeedbackService.Verify(s => s.SendFeedback("email", 5));
            component.Find("p.message").MarkupMatches("<p class=\"message\">feedback was sended succesfully</p>");
        }

        [Fact]
        public async Task ShouldDisplayError_IfEmailIsEmpty()
        {
            var component = RenderComponent<Feedback>();

            component.Find(".text input").Change("");
            component.Find(".stars-input input").Change(5);
            component.Find("button").Click();

            mockFeedbackService.Verify(s => s.SendFeedback("email", 5), Times.Never);
            component.Find("p.error").MarkupMatches("<p class=\"error\">email shouldn`t be empty</p>");

        }

        [Fact]
        public async Task ShouldDisplayError_IfGradeIncorrect()
        {
            var component = RenderComponent<Feedback>();

            component.Find(".text input").Change("email");
            component.Find(".stars-input input").Change(10);
            component.Find("button").Click();

            mockFeedbackService.Verify(s => s.SendFeedback("email", 5), Times.Never);
            component.Find("p.error").MarkupMatches("<p class=\"error\">grade must be between 1 and 5</p>");

        }
    }
}
