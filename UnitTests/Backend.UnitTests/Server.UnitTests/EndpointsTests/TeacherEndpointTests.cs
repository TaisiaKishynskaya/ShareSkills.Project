using System.Text.Json;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class TeacherEndpointTests
{
    private readonly Mock<ITeacherService> _teacherServiceMock;
    private readonly DefaultHttpContext _httpContext;

    public TeacherEndpointTests()
    {
        _teacherServiceMock = new Mock<ITeacherService>();

        // Создаем контекст для тестирования
        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(_teacherServiceMock.Object)
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .BuildServiceProvider()
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithTeachers()
    {
        // Arrange
        var expectedTeachers = new List<TeacherDto>
        {
            new TeacherDto
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Rating = 0,
                ClassTime = "Morning",
                Level = "Advanced",
                Skill = "Math"
            }
        };
        
        var cancellationToken = new CancellationToken();

        // Мокаем сервис, чтобы вернуть ожидаемый результат
        _teacherServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedTeachers);

        // Создаем контекст запроса и добавляем туда метод и путь
        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = "/teachers";

        // Создаем поток для записи ответа
        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        // Создаем делегат маршрута
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/teachers" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();
                var teachers = await service.GetAllAsync();

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(teachers);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _teacherServiceMock.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        // Проверяем содержимое ответа
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        // Проверяем корректность десериализации
        var responseTeachers = JsonSerializer.Deserialize<List<TeacherDto>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Игнорировать регистр имени свойств
        });

        Assert.NotNull(responseTeachers);
        Assert.Equal(expectedTeachers.Count, responseTeachers.Count);

        // Проверяем каждое свойство
        var expectedTeacher = expectedTeachers.First();
        var actualTeacher = responseTeachers.First();

        Assert.Equal(expectedTeacher.Id, actualTeacher.Id);
        Assert.Equal(expectedTeacher.UserId, actualTeacher.UserId);
        Assert.Equal(expectedTeacher.Rating, actualTeacher.Rating);
        Assert.Equal(expectedTeacher.ClassTime, actualTeacher.ClassTime);
        Assert.Equal(expectedTeacher.Level, actualTeacher.Level);
        Assert.Equal(expectedTeacher.Skill, actualTeacher.Skill);
    }
    
    [Fact]
    public async Task GetById_ReturnsOkResultWithTeacher()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var expectedTeacher = new TeacherExtendedDto 
        { 
            Id = teacherId,
            UserId = Guid.NewGuid(),
            Rating = 4,
            ClassTime = "DateTime.Now",
            Level = "Expert",
            Skill = "Math",
            Name = "Morning",
            Surname = "Advanced",
            Email = "Math"
        };
        
        var cancellationToken = new CancellationToken();

        // Мокаем сервис, чтобы вернуть ожидаемый результат
        _teacherServiceMock.Setup(service => service.GetByIdAsync(teacherId, cancellationToken))
            .ReturnsAsync(expectedTeacher);

        // Создаем контекст запроса и добавляем туда метод и путь
        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = $"/teachers/{teacherId}";

        // Создаем поток для записи ответа
        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        // Создаем делегат маршрута
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/teachers/{teacherId}" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();
                var teacher = await service.GetByIdAsync(teacherId, cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(teacher);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _teacherServiceMock.Verify(service => service.GetByIdAsync(teacherId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        // Проверяем содержимое ответа
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        // Проверяем корректность десериализации
        var responseTeacher = JsonSerializer.Deserialize<TeacherExtendedDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Игнорировать регистр имени свойств
        });

        Assert.NotNull(responseTeacher);

        // Проверяем каждое свойство
        Assert.Equal(expectedTeacher.Id, responseTeacher.Id);
        Assert.Equal(expectedTeacher.UserId, responseTeacher.UserId);
        Assert.Equal(expectedTeacher.Rating, responseTeacher.Rating);
        Assert.Equal(expectedTeacher.ClassTime, responseTeacher.ClassTime);
        Assert.Equal(expectedTeacher.Level, responseTeacher.Level);
        Assert.Equal(expectedTeacher.Skill, responseTeacher.Skill);
        Assert.Equal(expectedTeacher.Name, responseTeacher.Name);
        Assert.Equal(expectedTeacher.Surname, responseTeacher.Surname);
        Assert.Equal(expectedTeacher.Email, responseTeacher.Email);
    }
    
    [Fact]
    public async Task GetTeacherByEmail_ReturnsOkWithTeacherId()
    {
        // Arrange
        var email = "test@example.com";
        var expectedTeacherId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Set up the mock to return a specific teacher ID for the given email
        _teacherServiceMock.Setup(service => service.GetByEmailAsync(email, cancellationToken))
            .ReturnsAsync(expectedTeacherId);

        // Set up the HTTP context for a GET request
        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = $"/teachers/get-by-email/{email}";

        // Set up the request services
        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_teacherServiceMock.Object)
            .BuildServiceProvider();

        // Set up the response body stream
        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        // Define the request delegate
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/teachers/get-by-email/{email}" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();
                var teacherId = await service.GetByEmailAsync(email, cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(teacherId.ToString());
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _teacherServiceMock.Verify(service => service.GetByEmailAsync(email, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        // Check response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        // Deserialize the response body as a string
        var responseTeacherId = JsonSerializer.Deserialize<string>(responseBody);

        // Assert that the deserialized ID matches the expected ID
        Assert.Equal(expectedTeacherId.ToString(), responseTeacherId);
    }
    

    [Fact]
    public async Task CreateTeacher_ReturnsCreatedResultWithTeacher()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            UserId = default,
            Rating = 0,
            ClassTime = null,
            Level = null,
            Skill = null
        };

        var createdTeacher = new TeacherExtendedDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 0,
            ClassTime = null,
            Level = null,
            Skill = null,
            Name = null,
            Surname = null,
            Email = null
        };

        var cancellationToken = new CancellationToken();

        _teacherServiceMock.Setup(service => service.CreateAsync(teacherDto, cancellationToken))
            .ReturnsAsync(createdTeacher);

        // Set up the request
        _httpContext.Request.Method = HttpMethods.Post;
        _httpContext.Request.Path = "/teachers";

        // Serialize the request body
        using var requestBody = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBody, teacherDto);
        requestBody.Seek(0, SeekOrigin.Begin);
        _httpContext.Request.Body = requestBody;

        // Set up the request services
        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_teacherServiceMock.Object)
            .BuildServiceProvider();

        // Set up the response body stream
        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        // Define the request delegate
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/teachers" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<TeacherForCreationDto>(ctx.Request.Body);
                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();

                var teacher = await service.CreateAsync(dto, CancellationToken.None);

                ctx.Response.StatusCode = StatusCodes.Status201Created;
                ctx.Response.Headers["Location"] = $"/teachers/{teacher.Id}";
                await ctx.Response.WriteAsJsonAsync(teacher);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _teacherServiceMock.Verify(service => service.CreateAsync(teacherDto, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status201Created, _httpContext.Response.StatusCode);
        Assert.Equal($"/teachers/{createdTeacher.Id}", _httpContext.Response.Headers["Location"]);

        // Check response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var responseTeacher = JsonSerializer.Deserialize<TeacherExtendedDto>(responseBody);

        Assert.NotNull(responseTeacher);
        Assert.Equal(createdTeacher.Id, responseTeacher.Id);
        Assert.Equal(createdTeacher.Name, responseTeacher.Name);
        Assert.Equal(createdTeacher.Surname, responseTeacher.Surname);
        Assert.Equal(createdTeacher.Email, responseTeacher.Email);
    }
    
    [Fact]
    public async Task CreateTeacher_ReturnsBadRequest_OnException()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            UserId = default,
            Rating = 0,
            ClassTime = null,
            Level = null,
            Skill = null
        };

        var cancellationToken = new CancellationToken();

        // Mock the service to throw an exception
        _teacherServiceMock.Setup(service => service.CreateAsync(It.IsAny<TeacherForCreationDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid data"));

        // Create a new HTTP context
        var context = new DefaultHttpContext();
        context.Request.Path = "/teachers";
        context.Request.Method = HttpMethods.Post;

        // Serialize the request body
        var requestBodyStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBodyStream, teacherDto);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        // Set up the service provider
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_teacherServiceMock.Object)
            .BuildServiceProvider();

        // Create a stream for the response
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        // Create the request delegate
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/teachers" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<TeacherForCreationDto>(ctx.Request.Body, cancellationToken: ctx.RequestAborted);
                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();

                try
                {
                    var teacher = await service.CreateAsync(dto, ctx.RequestAborted);
                    ctx.Response.StatusCode = StatusCodes.Status201Created;
                    ctx.Response.Headers["Location"] = $"/teachers/{teacher.Id}";
                    await ctx.Response.WriteAsJsonAsync(teacher);
                }
                catch (InvalidOperationException ex)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsJsonAsync(new { StatusCode = 400, Message = ex.Message });
                }
            }
        };

        // Act
        await requestDelegate(context);

        // Assert
        _teacherServiceMock.Verify(service => service.CreateAsync(It.IsAny<TeacherForCreationDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        // Check the response body
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

        Assert.NotNull(responseJson);
        Assert.Equal("400", responseJson["StatusCode"]);
        Assert.Equal("Invalid data", responseJson["Message"]);
    }
    
    
    [Fact]
    public async Task DeleteTeacher_ReturnsNoContent()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Set up the mock to not throw any exceptions
        _teacherServiceMock.Setup(service => service.DeleteAsync(teacherId, cancellationToken))
            .Returns(Task.CompletedTask); // Task.CompletedTask is used for async methods that return void

        // Set up the HTTP context for a DELETE request
        _httpContext.Request.Method = HttpMethods.Delete;
        _httpContext.Request.Path = $"/teachers/{teacherId}";

        // Set up route values
        _httpContext.Request.RouteValues["id"] = teacherId.ToString();

        // Set up the request services
        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_teacherServiceMock.Object)
            .BuildServiceProvider();

        // Set up the response body stream
        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        // Define the request delegate
        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/teachers/{teacherId}" && ctx.Request.Method == HttpMethods.Delete)
            {
                // Convert route value from string to Guid
                var idString = ctx.Request.RouteValues["id"]?.ToString();
                if (!Guid.TryParse(idString, out var id))
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var service = ctx.RequestServices.GetRequiredService<ITeacherService>();

                await service.DeleteAsync(id, ctx.RequestAborted);

                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                await ctx.Response.CompleteAsync();
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _teacherServiceMock.Verify(service => service.DeleteAsync(teacherId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status204NoContent, _httpContext.Response.StatusCode);

        // Ensure response body is empty
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        Assert.Empty(responseBody);
    }
}