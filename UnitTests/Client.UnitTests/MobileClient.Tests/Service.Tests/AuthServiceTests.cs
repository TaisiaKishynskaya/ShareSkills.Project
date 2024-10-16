﻿using Moq;
using System.Net.Http.Json;
using System.Net;
using RichardSzalay.MockHttp;
using MobileClient.Services;



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
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false").Respond(req=>expectedResponse);
            var response = await authService.UserLogin("email@test.com", "123");

            // Assert
            Assert.True(response);
        }

        [Fact]
        public async Task UserLogin_ShouldSetJwt_IfResponseDoesntContainsCookies()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" })
            };
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false").Respond(req => expectedResponse);
            await authService.UserLogin("email@test.com", "123");
            mockPreferencesService.Verify(s => s.Set("jwt", "mock_token"));
        }

        [Fact]
        public async Task UserLogin_ShouldSetCookies_IfResponseContainsCookies()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" }),
            };
            expectedResponse.Headers.Add("Set-Cookie", "mock_cookie");
            mockPreferencesService.Setup(s => s.Get("allowCookies", String.Empty)).Returns("true");
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false").Respond(req => expectedResponse);
            await authService.UserLogin("email@test.com", "123");
            mockPreferencesService.Verify(s => s.Set("cookie", "mock_cookie"));
        }

        [Fact]
        public async Task UserLogin_ShouldNotSaveCookies_IfUserDenyUsingCookies()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" }),
            };
            expectedResponse.Headers.Add("Set-Cookie", "mock_cookie");
            mockPreferencesService.Setup(s => s.Get("allowCookies", String.Empty)).Returns("false");
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false").Respond(req => expectedResponse);
            await authService.UserLogin("email@test.com", "123");
            mockPreferencesService.Verify(s => s.Set("cookie", "mock_cookie"), Times.Never);
            mockPreferencesService.Verify(s => s.Set("jwt", "mock_token"));

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
        public async Task UserLogin_ShouldLoginWithJwt_IfHeadersDoesntContainCookies()
        {
            bool isCorrectUrl = false;
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" }),
            };
            //expectedResponse.Headers.Add("Set-Cookie", "mock_cookie");
            mockPreferencesService.Setup(s => s.Get("cookie", string.Empty)).Returns(string.Empty);
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false").Respond(req =>
            {
                if (req.RequestUri.ToString()== $"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=false")
                {
                    isCorrectUrl = true;
                }
                return expectedResponse;
            });
            await authService.UserLogin("email@test.com", "123");
            Assert.True(isCorrectUrl);
        }

        [Fact]
        public async Task UserLogin_ShouldLoginWithCookies_IfHeadersContainsCookies()
        {
            bool isCorrectUrl = false;
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" }),
            };
            //expectedResponse.Headers.Add("Set-Cookie", "mock_cookie");
            mockPreferencesService.Setup(s => s.Get("cookie", string.Empty)).Returns("mocked_cookie");
            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=true").Respond(req =>
            {
                if (req.RequestUri.ToString() == $"{fakeBaseAddres}/login?email=email@test.com&password=123&authMethodCookie=true")
                {
                    isCorrectUrl = true;
                }
                return expectedResponse;
            });
            await authService.UserLogin("email@test.com", "123");
            Assert.True(isCorrectUrl);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccesfulValidationResponse_WhenSuccessful()
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
            Assert.True(result.Succesful);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Register_ShouldReturnUnsuccesfulValidationResponse_WithErrors_WhenUnsuccessful()
        {
            // Arrange
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = JsonContent.Create(new ValidationResponse()
                {
                    Succesful = false,
                    Errors = new Dictionary<string, List<string>>()
                    {
                        { "Name", new List<string> { "name error" } }
                    }
                })
            };
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/register")
                .Respond(req => expectedResponse);

            // Act
            var result = await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123");

            // Assert
            Assert.False(result.Succesful);
            Assert.Equal("name error", result.Errors["Name"][0]);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Register_ShouldReturnUnsuccesfulValidationResponse_WithoutErrors_WhenThrowError()
        {
            // Arrange
            mockHttpMessageHandler
                .When(HttpMethod.Post, "http://localhost:5115/register")
                .Respond(req => throw new Exception());

            // Act
            var result = await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123");

            // Assert
            Assert.False(result.Succesful);
            Assert.Null(result.Errors);
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
