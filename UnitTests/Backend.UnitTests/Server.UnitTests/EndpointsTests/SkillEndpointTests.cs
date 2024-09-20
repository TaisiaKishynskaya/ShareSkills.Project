using System.Text.Json;
using App.Services.Abstract;
using Libraries.Contracts.Skill;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class SkillEndpointTests
{
    private readonly Mock<ISkillService> _skillServiceMock;
    private readonly DefaultHttpContext _httpContext;

    public SkillEndpointTests()
    {
        _skillServiceMock = new Mock<ISkillService>();

        // Создаем контекст для тестирования
        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(_skillServiceMock.Object)
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .BuildServiceProvider()
        };
    }
    
    [Fact]
    public async Task GetAll_ReturnsOkResultWithSkills()
    {
        // Arrange
        var expectedSkills = new List<SkillDto>
        {
            new() { Id = Guid.NewGuid(), Skill = "Math" }
        };
        
        var cancellationToken = new CancellationToken();

        _skillServiceMock.Setup(service => service.GetAllAsync(cancellationToken))
            .ReturnsAsync(expectedSkills);

        _httpContext.Request.Method = HttpMethods.Get;
        _httpContext.Request.Path = "/skills";

        var responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = responseBodyStream;

        RequestDelegate requestDelegate = async ctx =>
        {
            if (ctx.Request.Path == "/skills" && ctx.Request.Method == HttpMethods.Get)
            {
                var service = ctx.RequestServices.GetRequiredService<ISkillService>();
                var skills = await service.GetAllAsync(cancellationToken);

                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsJsonAsync(skills);
            }
        };

        _httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(_skillServiceMock.Object)
            .BuildServiceProvider();

        // Act
        await requestDelegate(_httpContext);

        // Assert
        _skillServiceMock.Verify(service => service.GetAllAsync(cancellationToken), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();

        var responseSkills = JsonSerializer.Deserialize<List<SkillDto>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(responseSkills);
        Assert.Equal(expectedSkills.Count, responseSkills.Count);

        var expectedSkill = expectedSkills.First();
        var actualSkill = responseSkills.First();

        Assert.Equal(expectedSkill.Id, actualSkill.Id);
        Assert.Equal(expectedSkill.Skill, actualSkill.Skill);
    }
}