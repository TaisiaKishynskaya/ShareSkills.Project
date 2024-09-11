using Moq;
using System.Net.Http.Json;
using System.Net;
using RichardSzalay.MockHttp;



namespace MobileClient.Tests.Service.Tests
{
    public class AuthServiceTests : TestsService
{
        private readonly AuthService authService;

        public AuthServiceTests()
        {
            mockPreferencesService.Setup(p => p.Get("jwt", It.IsAny<string>())).Returns("mocked_jwt");
            authService = new AuthService(httpClient, mockPreferencesService.Object);
        }

        [Fact]
        public async Task UserLogin_ShouldReturnTrue_WhenResponseIsSuccessful()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" })
            };

            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123").Respond(req=>expectedResponse);
            var response = await authService.UserLogin("email@test.com", "123");

            // Assert
            Assert.True(response);
        }


        [Fact]
        public async Task UserLogin_ShouldReturnFalse_WhenResponseIsUnsuccessful()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            
            };

            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123").Respond(req => expectedResponse);
            var response = await authService.UserLogin("email@test.com", "123");

            // Assert
            Assert.False(response);
        }

        [Fact]
        public async Task Register_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create("12345")
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/register")
                .Respond(req=>expectedResponse);

            // Act
            var result = await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123");

            // Assert
            Assert.True(result);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Register_ShouldReturnFalse_WhenUnsuccessful()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/register")
                .Respond(req => expectedResponse);

            // Act
            var result = await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123");

            // Assert
            Assert.False(result);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetSkills_ShouldReturnSkillsList_WhenSuccesful()
        {
            // Arrange
            var skillList = new List<Skill> { new Skill { id = "123", skill = "skill" } };
            var expectedResponseJson = JsonContent.Create(skillList);

            // Создаем мок ответа
            mockHttpMessageHandler.When($"{fakeBaseAddres}/skills").Respond(req =>
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(skillList)
                });

            // Act
            var response = await authService.GetSkills();

            // Теперь создаем ожидаемое значение заново, чтобы избежать повторного использования объекта
            var expectedResponse = await expectedResponseJson.ReadFromJsonAsync<List<Skill>>();

            // Assert
            Assert.Equal(expectedResponse.Count, response.Count);
            for (int i = 0; i < expectedResponse.Count; i++)
            {
                Assert.Equal(expectedResponse[i].id, response[i].id);
                Assert.Equal(expectedResponse[i].skill, response[i].skill);
            }

        }

        [Fact]
        public async Task GetSkills_ShouldReturnNull_WhenUnsuccesful()
        {
            // Arrange

            // Создаем мок ответа
            mockHttpMessageHandler.When($"{fakeBaseAddres}/skills").Respond(req =>
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                });

            // Act
            var response = await authService.GetSkills();

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task ChangeSkills_ShouldReturnTrue_WhenSuccesful()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/teachers")
                .Respond(req => expectedResponse);

            // Act
            var result = await authService.ChangeSkills("1", "skill", "time", "1");

            // Assert
            Assert.True(result);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ChangeSkills_ShouldReturnFalse_WhenUnsuccesful()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/teachers")
                .Respond(req => expectedResponse);

            // Act
            var result = await authService.ChangeSkills("1", "skill", "time", "1");

            // Assert
            Assert.False(result);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

    }
}
