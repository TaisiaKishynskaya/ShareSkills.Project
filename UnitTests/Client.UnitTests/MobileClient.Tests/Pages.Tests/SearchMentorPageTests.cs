using MobileClient.Components.Pages;
using MobileClient.Services;
using Bunit;
using Moq;
using Bunit.TestDoubles;

namespace MobileClient.Tests.Pages.Tests
{
    public class SearchMentorPageTests : TestContext
    {
        private Mock<IPreferencesService> mockPreferencesService;
        private Mock<ISearchService> mockSearchService;
        private Mock<IAuthService> mockAuthService;
        public SearchMentorPageTests()
        {
            mockPreferencesService = new Mock<IPreferencesService>();
            mockAuthService = new Mock<IAuthService>();
            mockSearchService = new Mock<ISearchService>();
            Services.AddSingleton<IPreferencesService>(mockPreferencesService.Object);
            Services.AddSingleton<IAuthService>(mockAuthService.Object);
            Services.AddSingleton<ISearchService>(mockSearchService.Object);
        }

        [Fact]
        public void Should_Render_Search_Input_And_Button()
        {
            // Arrange
            var cut = RenderComponent<SearchMentor>();

            // Act
            var searchInput = cut.Find("input[type='email']");
            var searchButton = cut.Find("button.find-btn");

            // Assert
            Assert.NotNull(searchInput);  // Email input is rendered
            Assert.NotNull(searchButton); // Search button is rendered
        }

        [Fact]
        public async Task Should_Display_Skills_When_Loaded()
        {
            // Arrange
            var skills = new List<Skill>
            {
                new Skill { skill = "C#" },
                new Skill { skill = "Python" }
            };
            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(skills);

            var cut = RenderComponent<SearchMentor>();
            cut.Find("svg").Click();

            // Assert
            Assert.Contains("C#", cut.Markup);
            Assert.Contains("Python", cut.Markup);
        }

        [Fact]
        public async Task Should_Search_Teacher_By_Email()
        {
            // Arrange
            var teacher = new Teacher { name = "John Doe", skill = "C#", rating = 5 };
            mockSearchService.Setup(service => service.GetTeacherByEmail(It.IsAny<string>())).ReturnsAsync(teacher);

            var cut = RenderComponent<SearchMentor>();

            // Act
            cut.Find("input[type='email']").Change("john.doe@example.com");
            cut.Find("button.find-btn").Click();

            // Assert
            mockSearchService.Verify(service => service.GetTeacherByEmail("john.doe@example.com"), Times.Once);
            Assert.Contains("John Doe", cut.Markup);
        }

        [Fact]
        public async Task Should_Search_Teacher_With_Filters()
        {
            // Arrange
            var teacher = new Teacher { name = "Jane Smith", skill = "Python", rating = 4.5 };
            var skills = new List<Skill>
            {
                new Skill { skill = "C#" },
                new Skill { skill = "Python" }
            };
            mockAuthService.Setup(service => service.GetSkills()).ReturnsAsync(skills);
            mockSearchService.Setup(service => service.SearchTeacher(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(teacher);

            var cut = RenderComponent<SearchMentor>();

            await cut.Find("svg").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            // Act
            cut.Find("input.skill-check[name='skill']").Change("Python");
            cut.Find("input.skill-check[name='time']").Change("Morning");
            cut.Find("input.skill-check[name='level']").Change("Advanced");
            cut.Find("button.find-btn").Click();

            // Assert
            Assert.Contains("Jane Smith", cut.Markup);
        }

        [Fact]
        public async Task Should_Navigate_To_Mentor_Info_On_Teacher_Card_Click()
        {
            // Arrange
            var teacher = new Teacher { id = "123", name = "John Doe", skill = "C#", rating = 5 };
            mockSearchService.Setup(service => service.GetTeacherByEmail(It.IsAny<string>())).ReturnsAsync(teacher);

            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            var cut = RenderComponent<SearchMentor>();

            // Act
            cut.Find("input[type='email']").Change("john.doe@example.com");
            cut.Find("button.find-btn").Click();
            cut.Find("div.card").Click();  // Simulate clicking the teacher card

            // Assert
            Assert.Equal("http://localhost/mentor/123", navMan.Uri);  // Ensure navigation to mentor info page
        }

        [Fact]
        public async Task Should_Display_Teacher_Results_After_Search()
        {
            // Arrange
            var teacher = new Teacher { name = "John Doe", skill = "C#", rating = 5 };
            mockSearchService.Setup(service => service.GetTeacherByEmail(It.IsAny<string>())).ReturnsAsync(teacher);

            var cut = RenderComponent<SearchMentor>();

            // Act
            cut.Find("input[type='email']").Change("john.doe@example.com");
            cut.Find("button.find-btn").Click();

            // Assert
            Assert.Contains("John Doe", cut.Markup);
            Assert.Contains("C#", cut.Markup);
            Assert.Contains("5", cut.Markup);
        }
    }
}
