using System.Text.Json;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Contracts.Grade;
using Libraries.Data;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Data.UnitOfWork.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server.UnitTests.EndpointsTests;

public class GradeEndpointTests
{
    private readonly Mock<IGradeService> _gradeServiceMock;
    private readonly DefaultHttpContext _httpContext;
    private readonly GradeEndpoint _endpoint;

    public GradeEndpointTests()
    {
        _gradeServiceMock = new Mock<IGradeService>();
        _endpoint = new GradeEndpoint();

        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(_gradeServiceMock.Object)
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .BuildServiceProvider()
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithGrades()
    {
        // Arrange
        var expectedGrades = new List<GradeDto>
        {
            new() { Id = Guid.NewGuid(), Grade = 1 }
        };
        
        var cancellationToken = new CancellationToken();

        _gradeServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedGrades);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = "/grades";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/grades" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IGradeService>();
                var grades = await service.GetAllAsync();

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(grades);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _gradeServiceMock.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseGrades = JsonSerializer.Deserialize<List<GradeDto>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(responseGrades);
        Assert.Equal(expectedGrades.Count, responseGrades.Count);

        var expectedGrade = expectedGrades.First();
        var actualGrade = responseGrades.First();

        Assert.Equal(expectedGrade.Id, actualGrade.Id);
        Assert.Equal(expectedGrade.Grade, actualGrade.Grade);
    }
    
    [Fact]
    public async Task GetById_ReturnsOkResultWithGrade()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var expectedGrade = new GradeDto { Id = gradeId, Grade = 1 };
        
        var cancellationToken = new CancellationToken();

        _gradeServiceMock.Setup(service => service.GetByIdAsync(gradeId, cancellationToken))
            .ReturnsAsync(expectedGrade);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = $"/grades/{gradeId}";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/grades/{gradeId}" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IGradeService>();
                var grade = await service.GetByIdAsync(gradeId, cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(grade);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _gradeServiceMock.Verify(service => service.GetByIdAsync(gradeId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseGrade = JsonSerializer.Deserialize<GradeDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(responseGrade);
        Assert.Equal(expectedGrade.Id, responseGrade.Id);
        Assert.Equal(expectedGrade.Grade, responseGrade.Grade);
    }
    
    [Fact]
    public async Task DeleteGrade_ReturnsNoContent()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _gradeServiceMock.Setup(service => service.DeleteAsync(gradeId, cancellationToken))
            .Returns(Task.CompletedTask); 

        _httpContext.Request.Method = HttpMethods.Delete;
        _httpContext.Request.Path = $"/grades/{gradeId}";

        _httpContext.Request.RouteValues["id"] = gradeId.ToString();

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_gradeServiceMock.Object)
            .BuildServiceProvider();

        _httpContext.Response.Body = new MemoryStream();

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/grades/{gradeId}" && ctx.Request.Method == HttpMethods.Delete)
            {
                var idString = ctx.Request.RouteValues["id"]?.ToString();
                if (!Guid.TryParse(idString, out var id))
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var service = ctx.RequestServices.GetRequiredService<IGradeService>();

                await service.DeleteAsync(id, ctx.RequestAborted);

                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                await ctx.Response.CompleteAsync();
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _gradeServiceMock.Verify(service => service.DeleteAsync(gradeId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status204NoContent, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        Assert.Empty(responseBody);
    }
    
    [Fact]
    public async Task CreateGrade_ReturnsCreatedResultWithGrade()
    {
        // Arrange
        var gradeDto = new GradeForCreatingDto { Grade = 0 };
        Mock<IUnitOfWork> ufMock = new Mock<IUnitOfWork>();
        Mock<ITeacherService> tsMock = new();

        ufMock.Setup(uow => uow.GradeRepository.Insert(new GradeEntity
        {
            Id = default
        }));
        ufMock.Setup(x => x.TeacherRepository.GetScoresByTeacherIdAsync(It.IsAny<Guid>())).ReturnsAsync(new int[10]);
        
        var service = new GradeService(ufMock.Object, tsMock.Object);
        // Act

        var grade = await service.CreateAsync(gradeDto, CancellationToken.None);
        // Assert

        Assert.Equal(0, grade.Grade);
        Assert.NotNull(grade);
    }
    
    [Fact]
    public async Task CreateGrade_ReturnsBadRequest_OnException()
    {
        // Arrange
        var gradeDto = new GradeForCreatingDto { Grade = 0 };

        var cancellationToken = new CancellationToken();

        _gradeServiceMock.Setup(service => service.CreateAsync(It.IsAny<GradeForCreatingDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid data"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/grades";
        context.Request.Method = HttpMethods.Post;

        var requestBodyStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBodyStream, gradeDto);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        context.RequestServices = new ServiceCollection()
            .AddSingleton(_gradeServiceMock.Object)
            .BuildServiceProvider();

        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/grades" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<GradeForCreatingDto>(ctx.Request.Body, cancellationToken: ctx.RequestAborted);
                var service = ctx.RequestServices.GetRequiredService<IGradeService>();

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
        _gradeServiceMock.Verify(service => service.CreateAsync(It.IsAny<GradeForCreatingDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var responseJson = JsonConvert.DeserializeObject<ErrorRepresentation>(responseBody);

        Assert.NotNull(responseJson);
        Assert.Equal(400, responseJson.StatusCode);
        Assert.Equal("Invalid data", responseJson.Message);
    }
    
    
    [Fact]
    public async Task Put_UpdateGrade_ReturnsNoContent()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var gradeForUpdate = new GradeForUpdateDto
        {
            Grade = 95
        };

        // Mock the service method to verify it gets called
        _gradeServiceMock.Setup(service => service.UpdateAsync(gradeId, gradeForUpdate, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();  // Ensuring the method gets called

        // Act
        // Simulating the endpoint call directly
        var result = await new Func<Guid, GradeForUpdateDto, IGradeService, Task<IResult>>(async (id, dto, service) =>
        {
            await service.UpdateAsync(id, dto);
            return Results.NoContent();
        })(gradeId, gradeForUpdate, _gradeServiceMock.Object);

        // Assert
        _gradeServiceMock.Verify(service => service.UpdateAsync(gradeId, gradeForUpdate, It.IsAny<CancellationToken>()), Times.Once);
    
        // Verifying the result is NoContent
        var noContentResult = Assert.IsType<NoContent>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}

public class ErrorRepresentation
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}