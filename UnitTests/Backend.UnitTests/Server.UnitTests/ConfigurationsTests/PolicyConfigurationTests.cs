using App.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Server.UnitTests.ConfigurationsTests;

public class PolicyConfigurationTests
{
    [Fact]
    public void ConfigureCors_ShouldAddCorsPolicy()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        PolicyConfiguration.ConfigureCors(builder);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var corsPolicyProvider = serviceProvider.GetService<ICorsPolicyProvider>();
        Assert.NotNull(corsPolicyProvider);

        // Ensure that the "AllowAll" policy is configured
        var corsPolicyOptions = serviceProvider.GetService<IOptions<CorsOptions>>()?.Value;
        Assert.NotNull(corsPolicyOptions);

        var corsPolicy = corsPolicyOptions.GetPolicy("AllowAll");
        Assert.NotNull(corsPolicy);

        // Validate the settings of the "AllowAll" policy
        Assert.Contains("https://localhost:7281", corsPolicy.Origins);  // Проверить, что разрешён конкретный домен
        Assert.True(corsPolicy.AllowAnyMethod);  // Проверить, что разрешены любые методы
        Assert.True(corsPolicy.AllowAnyHeader);  // Проверить, что разрешены любые заголовки
        Assert.True(corsPolicy.SupportsCredentials);  // Проверить, что разрешены куки
    }
}
