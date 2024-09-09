using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Net.Http.Json;
using System.Net;
using Moq.Protected;
using RichardSzalay.MockHttp;
using System.Net.Http;

namespace MobileClient.Tests.Service.Tests
{
    public class AuthServiceTests
{
    private readonly AuthService authService;
    private readonly MockHttpMessageHandler mockHttpMessageHandler;
    private string fakeBaseAddres;
        private HttpClient httpClient;

        public AuthServiceTests()
        {
            fakeBaseAddres = "http://localhost:5115";
            mockHttpMessageHandler = new MockHttpMessageHandler();
            httpClient = new HttpClient(mockHttpMessageHandler);
            authService = new AuthService(httpClient);
        }

        [Fact]
        public async Task UserLogin_ShouldReturnTrue_WhenResponseIsSuccessful()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AuthResponse { token = "mock_token", userId = "mock_user" })
            };

            mockHttpMessageHandler.When($"{fakeBaseAddres}/login?email=email@test.com&password=123").Respond(req=>expectedResponse);
            var response = await authService.UserLogin("email@test.com", "123", true);

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
            var response = await authService.UserLogin("email@test.com", "123", true);

            // Assert
            Assert.False(response);
        }

        //[Fact]
        //public async Task Register_ShouldPostRequest_WhithCorrectData()
        //{
        //    // Arrange
        //    var expectedRequestBody = new
        //    {
        //        Name = "John",
        //        Surname = "Doe",
        //        Email = "john.doe@example.com",
        //        Password = "Password123",
        //        Role = "teacher"
        //    };

        //    mockHttpMessageHandler
        //        .When(HttpMethod.Post, "http://localhost:5115/register")
        //        .WithContent(expectedRequestBody)
        //        .RespondJson("12345");

        //    // Act
        //    await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123");

        //    // Assert
        //    var request = mockHttpMessageHandler.GetMatch(HttpMethod.Post, "http://localhost:5115/register");
        //    var actualRequestBody = await request.Content.ReadFromJsonAsync<dynamic>();

        //    Assert.NotNull(actualRequestBody);
        //    Assert.Equal(expectedRequestBody.Name, (string)actualRequestBody.Name);
        //    Assert.Equal(expectedRequestBody.Surname, (string)actualRequestBody.Surname);
        //    Assert.Equal(expectedRequestBody.Email, (string)actualRequestBody.Email);
        //    Assert.Equal(expectedRequestBody.Password, (string)actualRequestBody.Password);
        //    Assert.Equal(expectedRequestBody.Role, (string)actualRequestBody.Role);

        //    mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        //}

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
            var result = await authService.Register(true, "John", "Doe", "john.doe@example.com", "Password123", true);

            // Assert
            Assert.True(result);
            mockHttpMessageHandler.VerifyNoOutstandingExpectation();
        }

    }
}
