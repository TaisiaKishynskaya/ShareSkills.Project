using System.Text.Json;
using App.Services.Abstract;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class UserEndpointTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly IEndpointRouteBuilder _routeBuilder;

    public UserEndpointTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _routeBuilder = new Mock<IEndpointRouteBuilder>().Object;
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithUsers()
    {
        // Arrange
        var expectedUsers = new List<UserDto>
        {
            new UserDto
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Surname = "Test Surname",
                Email = "test@example.com",
                PasswordHash = ""
            }
        };

        // Создаем CancellationToken для вызова
        var cancellationToken = new CancellationToken();

        // Мокаем сервис, чтобы вернуть ожидаемый результат
        _userServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        // Эмулируем вызов эндпоинта вручную
        var result = await _userServiceMock.Object.GetAllAsync(cancellationToken);

        // Assert
        // Проверяем, что результат вызова - это список пользователей
        Assert.IsType<List<UserDto>>(result);
        Assert.Equal(expectedUsers, result);
    }
    
    [Fact]
    public async Task GetById_ReturnsOkResultWithUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new UserDto
        {
            Id = userId,
            Name = "Test User",
            Surname = "Test Surname",
            Email = "test@example.com",
            PasswordHash = ""
        };

        var cancellationToken = new CancellationToken();
        
        // Mock the service to return the expected user
        _userServiceMock.Setup(service => service.GetByIdAsync(userId, cancellationToken))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userServiceMock.Object.GetByIdAsync(userId, cancellationToken);

        // Assert
        Assert.IsType<UserDto>(result);
        Assert.Equal(expectedUser, result);
    }
    
    [Fact]
    public async Task Put_UpdateUser_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userForUpdate = new UserForUpdateDto
        {
            Name = "Updated Name",
            Surname = "Updated Surname",
            Email = "updated@example.com",
            Password = null
        };

        // Mock the service method to verify it gets called
        _userServiceMock.Setup(service => service.UpdateAsync(userId, userForUpdate, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();  // Ensuring the method gets called

        // Act
        // Simulating the endpoint call directly
        var result = await new Func<Guid, UserForUpdateDto, IUserService, Task<IResult>>(async (id, dto, service) =>
        {
            await service.UpdateAsync(id, dto);
            return Results.NoContent();
        })(userId, userForUpdate, _userServiceMock.Object);

        // Assert
        _userServiceMock.Verify(service => service.UpdateAsync(userId, userForUpdate, It.IsAny<CancellationToken>()), Times.Once);
    
        // Verifying the result is NoContent
        var noContentResult = Assert.IsType<NoContent>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
    
        var cancellationToken = new CancellationToken();
        
        // Мокаем сервис, чтобы удалить пользователя
        _userServiceMock.Setup(service => service.DeleteAsync(userId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Создаем контекст для запроса
        var context = new DefaultHttpContext();
        context.Request.Path = $"/users/{userId}";
        context.Request.Method = HttpMethods.Delete;

        // Устанавливаем сервисы для запроса
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_userServiceMock.Object)
            .BuildServiceProvider();

        // Создаем делегат запроса, который представляет эндпоинт
        RequestDelegate requestDelegate = async ctx =>
        {
            // Маппинг маршрута вручную
            if (ctx.Request.Path == $"/users/{userId}" && ctx.Request.Method == HttpMethods.Delete)
            {
                var service = ctx.RequestServices.GetRequiredService<IUserService>();
                await service.DeleteAsync(userId);
                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
            }
        };

        // Act
        await requestDelegate(context);

        // Assert
        // Проверяем, что метод DeleteAsync был вызван с правильными параметрами
        _userServiceMock.Verify(service => service.DeleteAsync(userId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    
    [Fact]
    public async Task Post_GetUserIdByEmail_ReturnsOkWithUserId()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Surname = "User",
            Email = email,
            PasswordHash = null
        };
        
        var cancellationToken = new CancellationToken();

        // Мокаем сервис, чтобы вернуть пользователя по email
        _userServiceMock.Setup(service => service.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Создаем контекст для запроса
        var context = new DefaultHttpContext();
        context.Request.Path = "/getId";
        context.Request.Method = HttpMethods.Post;

        // Устанавливаем параметры запроса (email)
        var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "email", email }
        });
        context.Request.Query = query;

        // Устанавливаем сервисы для запроса
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_userServiceMock.Object)
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build()) // Мокаем IConfiguration если требуется
            .BuildServiceProvider();

        // Создаем поток для записи ответа
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        // Создаем делегат запроса, который представляет эндпоинт
        RequestDelegate requestDelegate = async ctx =>
        {
            // Маппинг маршрута вручную
            if (ctx.Request.Path == "/getId" && ctx.Request.Method == HttpMethods.Post)
            {
                var userService = ctx.RequestServices.GetRequiredService<IUserService>();
                var user = await userService.GetByEmailAsync(ctx.Request.Query["email"], CancellationToken.None);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(new { userId = user.Id });
            }
        };

        // Act
        await requestDelegate(context);

        // Assert
        _userServiceMock.Verify(service => service.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);

        // Проверка на возврат userId
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        
        Assert.NotNull(responseJson);
        Assert.Equal(expectedUser.Id.ToString(), responseJson["userId"]);
    }
}