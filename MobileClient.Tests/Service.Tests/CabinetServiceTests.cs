using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;
using MobileClient.Services;
namespace MobileClient.Tests.Service.Tests;
    public class CabinetServiceTests : TestsService
{
    private readonly CabinetService _cabinetService;

    public CabinetServiceTests()
    {
        mockPreferencesService.Setup(p => p.Get("userId", It.IsAny<string>())).Returns("1");
        _cabinetService = new CabinetService(httpClient, mockPreferencesService.Object);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenSuccesful()
    {
        var user = new User { Id = "1", Name = "name", Surname = "surname", Email = "email", PasswordHash = "pass", Role = "student" };
        var expectedResponseJson = JsonContent.Create(user);

        mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(user)
                });

        var response = await _cabinetService.GetUser();
        var expectedResponse = await expectedResponseJson.ReadFromJsonAsync<User>();

        Assert.Equal(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async Task GetUser_ShouldReturnNull_WhenUnsuccesful()
    {

        mockHttpMessageHandler
                .When(HttpMethod.Get, $"{fakeBaseAddres}/users/1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                });

        var response = await _cabinetService.GetUser();

        Assert.Null(response);
    }

    [Fact]
    public async Task ChangeInfo_ShouldReturnTrue_WhenSuccesful()
    {
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
        };
        // Arrange
        mockHttpMessageHandler
            .When(HttpMethod.Put, $"{fakeBaseAddres}/users/1")
            .Respond(req => expectedResponse);

        // Act
        var result = await _cabinetService.ChangeInfo(new User { Id = "1", Name = "name", Surname = "surname", Email = "email", PasswordHash = "pass", Role = "student" }, "newPass");

        // Assert
        Assert.True(result);
        mockHttpMessageHandler.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ChangeInfo_ShouldReturnFalse_WhenUnsuccesful()
    {
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
        };
        // Arrange
        mockHttpMessageHandler
            .When(HttpMethod.Put, $"{fakeBaseAddres}/users/1")
            .Respond(req => expectedResponse);

        // Act
        var result = await _cabinetService.ChangeInfo(new User { Id = "1", Name = "name", Surname = "surname", Email = "email", PasswordHash = "pass", Role = "student" }, "newPass");

        // Assert
        Assert.False(result);
        mockHttpMessageHandler.VerifyNoOutstandingExpectation();
    }
}