using App.Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Server.UnitTests.ConfigurationsTests;


public class SwaggerConfigurationTests
{
    [Fact]
    public void AddSwagger_ShouldAddSwaggerGenServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build(); // You can use an empty configuration for this test

        // Act
        services.AddSwagger(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var swaggerGenOptions = serviceProvider.GetService<IOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwagger_ShouldConfigureSwaggerWithSecurityDefinition()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build(); // You can use an empty configuration for this test

        // Act
        services.AddSwagger(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

        // Assert
        Assert.NotNull(swaggerGenOptions);
        
        // Ensure the security definition is configured
        var securityScheme = swaggerGenOptions.SwaggerGeneratorOptions.SecuritySchemes
            .FirstOrDefault(s => s.Key == "Bearer").Value;

        Assert.NotNull(securityScheme);
        Assert.Equal("Bearer", securityScheme.Scheme);
        Assert.Equal("JWT", securityScheme.BearerFormat);
    }
}
