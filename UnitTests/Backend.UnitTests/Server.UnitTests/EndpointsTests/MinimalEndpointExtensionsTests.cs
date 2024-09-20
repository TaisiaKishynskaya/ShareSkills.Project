using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Server.UnitTests.EndpointsTests;

public class MinimalEndpointExtensionsTests
{
    private IServiceCollection _services;

    public MinimalEndpointExtensionsTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddMinimalEndpoints_ShouldRegisterMinimalEndpoints()
    {
        // Arrange
        _services.AddMinimalEndpoints();

        // Act
        var serviceProvider = _services.BuildServiceProvider();

        // Assert - Check if services that implement IMinimalEndpoint are registered
        var minimalEndpoints = serviceProvider.GetServices<IMinimalEndpoint>();
        Assert.NotEmpty(minimalEndpoints);
    }
    
    [Fact]
    public void RegisterMinimalEndpoints_ShouldCallMapRoutesForAllEndpoints()
    {
        // Arrange
        var mockEndpoint1 = new Mock<IMinimalEndpoint>();
        var mockEndpoint2 = new Mock<IMinimalEndpoint>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockEndpoint1.Object);
        serviceCollection.AddSingleton(mockEndpoint2.Object);
        
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSingleton(mockEndpoint1.Object);
        builder.Services.AddSingleton(mockEndpoint2.Object);

        var app = builder.Build();

        // Act
        app.RegisterMinimalEndpoints();

        // Assert
        mockEndpoint1.Verify(e => e.MapRoutes(It.IsAny<IEndpointRouteBuilder>()), Times.Once);
        mockEndpoint2.Verify(e => e.MapRoutes(It.IsAny<IEndpointRouteBuilder>()), Times.Once);
    }
}
