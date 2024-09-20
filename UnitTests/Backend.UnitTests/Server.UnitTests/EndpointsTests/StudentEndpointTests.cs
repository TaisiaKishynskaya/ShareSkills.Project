using System.Text.Json;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Contracts.Student;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server.UnitTests.EndpointsTests;

public class StudentEndpointTests
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly DefaultHttpContext _httpContext;

    public StudentEndpointTests()
    {
        _studentServiceMock = new Mock<IStudentService>();

        // Создаем контекст для тестирования
        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(_studentServiceMock.Object)
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .BuildServiceProvider()
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithStudents()
    {
        // Arrange
        var expectedStudents = new List<StudentDto>
        {
            new() { Id = Guid.NewGuid(), }
        };
        
        var cancellationToken = new CancellationToken();

        _studentServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedStudents);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = "/students";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/students" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IStudentService>();
                var students = await service.GetAllAsync();

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(students);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _studentServiceMock.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseStudents = JsonSerializer.Deserialize<List<StudentDto>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(responseStudents);
        Assert.Equal(expectedStudents.Count, responseStudents.Count);

        var expectedStudent = expectedStudents.First();
        var actualStudent = responseStudents.First();

        Assert.Equal(expectedStudent.Id, actualStudent.Id);
    }
    
    [Fact]
    public async Task GetById_ReturnsOkResultWithStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var expectedStudent = new StudentDto 
        { 
            Id = studentId,
        };
        
        var cancellationToken = new CancellationToken();

        _studentServiceMock.Setup(service => service.GetByIdAsync(studentId, cancellationToken))
            .ReturnsAsync(expectedStudent);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = $"/students/{studentId}";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/students/{studentId}" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IStudentService>();
                var student = await service.GetByIdAsync(studentId, cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(student);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _studentServiceMock.Verify(service => service.GetByIdAsync(studentId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseStudent = JsonSerializer.Deserialize<StudentDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(responseStudent);

        Assert.Equal(expectedStudent.Id, responseStudent.Id);
    }
    

    [Fact]
    public async Task CreateStudent_ReturnsCreatedResultWithStudent()
    {
        // Arrange
        var studentDto = new StudentForCreationDto();

        var cancellationToken = new CancellationToken();

        var ufMock = new Mock<IUnitOfWork>();

        ufMock.Setup(x => x.StudentRepository.Insert(It.IsAny<StudentEntity>()));

        var service = new StudentService(ufMock.Object);

        // Act

        var student = await service.CreateAsync(studentDto, CancellationToken.None);
        // Assert
        Assert.NotNull(student);
    }
    
    [Fact]
    public async Task CreateStudent_ReturnsBadRequest_OnException()
    {
        // Arrange
        var gradeDto = new StudentForCreationDto()
        {
        };

        var cancellationToken = new CancellationToken();

        _studentServiceMock.Setup(service => service.CreateAsync(It.IsAny<StudentForCreationDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid data"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/grades";
        context.Request.Method = HttpMethods.Post;

        var requestBodyStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBodyStream, gradeDto);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        context.RequestServices = new ServiceCollection()
            .AddSingleton(_studentServiceMock.Object)
            .BuildServiceProvider();

        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/grades" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<StudentForCreationDto>(ctx.Request.Body, cancellationToken: ctx.RequestAborted);
                var service = ctx.RequestServices.GetRequiredService<IStudentService>();

                try
                {
                    var grade = await service.CreateAsync(dto, ctx.RequestAborted);
                    ctx.Response.StatusCode = StatusCodes.Status201Created;
                    ctx.Response.Headers["Location"] = $"/grades/{grade.Id}";
                    await ctx.Response.WriteAsJsonAsync(grade);
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
        _studentServiceMock.Verify(service => service.CreateAsync(It.IsAny<StudentForCreationDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var responseJson = JsonConvert.DeserializeObject<ErrorRepresentation>(responseBody);

        Assert.NotNull(responseJson);
        Assert.Equal(400, responseJson.StatusCode);
        Assert.Equal("Invalid data", responseJson.Message);
    }
    
    
    [Fact]
    public async Task DeleteStudent_ReturnsNoContent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _studentServiceMock.Setup(service => service.DeleteAsync(studentId, cancellationToken))
            .Returns(Task.CompletedTask); 

        _httpContext.Request.Method = HttpMethods.Delete;
        _httpContext.Request.Path = $"/students/{studentId}";

        _httpContext.Request.RouteValues["id"] = studentId.ToString();

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_studentServiceMock.Object)
            .BuildServiceProvider();

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/students/{studentId}" && ctx.Request.Method == HttpMethods.Delete)
            {
                var idString = ctx.Request.RouteValues["id"]?.ToString();
                if (!Guid.TryParse(idString, out var id))
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var service = ctx.RequestServices.GetRequiredService<IStudentService>();

                await service.DeleteAsync(id, ctx.RequestAborted);

                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                await ctx.Response.CompleteAsync();
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _studentServiceMock.Verify(service => service.DeleteAsync(studentId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status204NoContent, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        Assert.Empty(responseBody);
    }
}