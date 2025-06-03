using System.Net;
using System.Net.Http.Json;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.RecommendationSystem.Test;
using Libraries.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Server.UnitTests.Test;

public class RecommendationsEndpointTests
{
    private readonly IHost _host;
    private readonly HttpClient _client;
    private readonly AppDbContext _ctx;

    public RecommendationsEndpointTests()
    {
        // Используем тот же FakeAppDbContext для проверки совпадения
        _ctx = new AppDbContext(new DbContextOptions<AppDbContext>());

        _host = new HostBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer()
                          .Configure(app =>
                          {
                              app.UseRouting();
                              app.UseEndpoints(endpoints =>
                              {
                                  endpoints.RegisterRecommendationEndpoint();
                              });
                          });
            })
            .Start();

        _client = _host.GetTestServer().CreateClient();
    }

    [Fact]
    public async Task RecTeachers_ReturnsOk_WithExpectedCount_And_Guids()
    {
        // Act
        var response = await _client.GetAsync("/rec-teachers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var guids = await response.Content.ReadFromJsonAsync<List<Guid>>();
        Assert.NotNull(guids);
        // Должно вернуть ровно 3 элемента
        Assert.Equal(2, guids.Count);
        // Все значения должны быть из контекста _ctx.Teachers
        var validIds = _ctx.Teachers.Select(t => t.Id).ToHashSet();
        Assert.All(guids, g => Assert.Contains(g, validIds));
    }

    [Fact]
    public async Task RecCourses_ReturnsOk_WithExpectedCount_And_Names()
    {
        // Act
        var response = await _client.GetAsync("/rec-courses");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(names);
        // Должно вернуть ровно 3 элемента
        Assert.Equal(3, names.Count);
        // Все имена должны быть из контекста _ctx.Courses
        var validNames = _ctx.Courses.Select(c => c.Name).ToHashSet();
        Assert.All(names, n => Assert.Contains(n, validNames));
    }

    [Fact]
    public async Task InvalidEndpoint_ReturnsNotFound()
    {
        var resp = await _client.GetAsync("/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}