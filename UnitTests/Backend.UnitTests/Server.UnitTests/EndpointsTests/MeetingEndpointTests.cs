using System.Text.Json;
using App.Services.Abstract;
using Libraries.Contracts.Meeting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class MeetingEndpointTests
{
    private readonly Mock<IMeetingService> _meetingServiceMock;
    private readonly DefaultHttpContext _httpContext;

    public MeetingEndpointTests()
    {
        _meetingServiceMock = new Mock<IMeetingService>();

        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(_meetingServiceMock.Object)
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .BuildServiceProvider()
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithMeetings()
    {
        // Arrange
        var expectedMeetings = new List<MeetingDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = null,
                DateTime = default,
                OwnerId = default,
                ForeignId = default
            }
        };
        
        var cancellationToken = new CancellationToken();

        _meetingServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedMeetings);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = "/meetings";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/meetings" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IMeetingService>();
                var meetings = await service.GetAllAsync();

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(meetings);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _meetingServiceMock.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseMeetings = JsonSerializer.Deserialize<List<MeetingDto>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(responseMeetings);
        Assert.Equal(expectedMeetings.Count, responseMeetings.Count);

        var expectedMeeting = expectedMeetings.First();
        var actualMeeting = responseMeetings.First();

        Assert.Equal(expectedMeeting.Id, actualMeeting.Id);
        Assert.Equal(expectedMeeting.Name, actualMeeting.Name);
        Assert.Equal(expectedMeeting.DateTime, actualMeeting.DateTime);
        Assert.Equal(expectedMeeting.OwnerId, actualMeeting.OwnerId);
        Assert.Equal(expectedMeeting.ForeignId, actualMeeting.ForeignId);
    }
    
    [Fact]
    public async Task GetById_ReturnsOkResultWithMeeting()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var expectedMeeting = new MeetingDto
        {
            Id = meetingId,
            Name = "Morning",
            DateTime = default,
            OwnerId = default,
            ForeignId = default
        };
        
        var cancellationToken = new CancellationToken();

        _meetingServiceMock.Setup(service => service.GetByIdAsync(meetingId, cancellationToken))
            .ReturnsAsync(expectedMeeting);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = $"/meetings/{meetingId}";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/meetings/{meetingId}" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<IMeetingService>();
                var meeting = await service.GetByIdAsync(meetingId, cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(meeting);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _meetingServiceMock.Verify(service => service.GetByIdAsync(meetingId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseMeeting = JsonSerializer.Deserialize<MeetingDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });

        Assert.NotNull(responseMeeting);
        Assert.Equal(expectedMeeting.Id, responseMeeting.Id);
        Assert.Equal(expectedMeeting.DateTime, responseMeeting.DateTime);
        Assert.Equal(expectedMeeting.OwnerId, responseMeeting.OwnerId);
        Assert.Equal(expectedMeeting.ForeignId, responseMeeting.ForeignId);
        Assert.Equal(expectedMeeting.Name, responseMeeting.Name);
    }
    

    [Fact]
    public async Task CreateMeeting_ReturnsCreatedResultWithMeeting()
    {
        // Arrange
        var meetingDto = new MeetingForCreatingDto
        {
            Name = null,
            DateAndTime = default,
            OwnerId = default,
            ForeignId = default
        };

        var createdMeeting = new MeetingDto
        {
            Id = Guid.NewGuid(),
            Name = null,
            DateTime = default,
            OwnerId = default,
            ForeignId = default
        };

        var cancellationToken = new CancellationToken();

        _meetingServiceMock.Setup(service => service.TryToCreateAsync(meetingDto, cancellationToken))
            .ReturnsAsync(createdMeeting);

        _httpContext.Request.Method = HttpMethods.Post;
        _httpContext.Request.Path = "/meetings";

        using var requestBody = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBody, meetingDto);
        requestBody.Seek(0, SeekOrigin.Begin);
        _httpContext.Request.Body = requestBody;

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_meetingServiceMock.Object)
            .BuildServiceProvider();

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/meetings" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<MeetingForCreatingDto>(ctx.Request.Body);
                var service = ctx.RequestServices.GetRequiredService<IMeetingService>();

                var meeting = await service.TryToCreateAsync(dto, CancellationToken.None);

                ctx.Response.StatusCode = StatusCodes.Status201Created;
                ctx.Response.Headers["Location"] = $"/meetings/{meeting.Id}";
                await ctx.Response.WriteAsJsonAsync(meeting);
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _meetingServiceMock.Verify(service => service.TryToCreateAsync(meetingDto, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status201Created, _httpContext.Response.StatusCode);
        Assert.Equal($"/meetings/{createdMeeting.Id}", _httpContext.Response.Headers["Location"]);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var responseMeeting = JsonSerializer.Deserialize<MeetingDto>(responseBody);

        Assert.NotNull(responseMeeting);
        Assert.Equal(createdMeeting.Id, responseMeeting.Id);
        Assert.Equal(createdMeeting.Name, responseMeeting.Name);
        Assert.Equal(createdMeeting.DateTime, responseMeeting.DateTime);
        Assert.Equal(createdMeeting.OwnerId, responseMeeting.OwnerId);
        Assert.Equal(createdMeeting.ForeignId, responseMeeting.ForeignId);
    }
    
    [Fact]
    public async Task CreateMeeting_ReturnsBadRequest_OnException()
    {
        // Arrange
        var meetingDto = new MeetingForCreatingDto
        {
            Name = null,
            DateAndTime = default,
            OwnerId = default,
            ForeignId = default
        };

        var cancellationToken = new CancellationToken();

        _meetingServiceMock.Setup(service => service.TryToCreateAsync(It.IsAny<MeetingForCreatingDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid data"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/meetings";
        context.Request.Method = HttpMethods.Post;

        var requestBodyStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(requestBodyStream, meetingDto);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        context.RequestServices = new ServiceCollection()
            .AddSingleton(_meetingServiceMock.Object)
            .BuildServiceProvider();

        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/meetings" && ctx.Request.Method == HttpMethods.Post)
            {
                var dto = await JsonSerializer.DeserializeAsync<MeetingForCreatingDto>(ctx.Request.Body, cancellationToken: ctx.RequestAborted);
                var service = ctx.RequestServices.GetRequiredService<IMeetingService>();

                try
                {
                    var meeting = await service.TryToCreateAsync(dto, ctx.RequestAborted);
                    ctx.Response.StatusCode = StatusCodes.Status201Created;
                    ctx.Response.Headers["Location"] = $"/meetings/{meeting.Id}";
                    await ctx.Response.WriteAsJsonAsync(meeting);
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
        _meetingServiceMock.Verify(service => service.TryToCreateAsync(It.IsAny<MeetingForCreatingDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

        Assert.NotNull(responseJson);
        Assert.Equal("400", responseJson["StatusCode"]);
        Assert.Equal("Invalid data", responseJson["Message"]);
    }
    
    
    [Fact]
    public async Task DeleteMeeting_ReturnsNoContent()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _meetingServiceMock.Setup(service => service.DeleteAsync(meetingId, cancellationToken))
            .Returns(Task.CompletedTask); 

        _httpContext.Request.Method = HttpMethods.Delete;
        _httpContext.Request.Path = $"/meetings/{meetingId}";

        _httpContext.Request.RouteValues["id"] = meetingId.ToString();

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_meetingServiceMock.Object)
            .BuildServiceProvider();

        _httpContext.Response.Body = new MemoryStream();

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == $"/meetings/{meetingId}" && ctx.Request.Method == HttpMethods.Delete)
            {
                var idString = ctx.Request.RouteValues["id"]?.ToString();
                if (!Guid.TryParse(idString, out var id))
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var service = ctx.RequestServices.GetRequiredService<IMeetingService>();

                await service.DeleteAsync(id, ctx.RequestAborted);

                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                await ctx.Response.CompleteAsync();
            }
        };

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _meetingServiceMock.Verify(service => service.DeleteAsync(meetingId, cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status204NoContent, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        Assert.Empty(responseBody);
    }
}