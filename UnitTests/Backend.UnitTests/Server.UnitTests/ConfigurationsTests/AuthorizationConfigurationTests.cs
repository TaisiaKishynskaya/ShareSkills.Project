using App.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Server.UnitTests.ConfigurationsTests;

public class AuthorizationConfigurationTests
{
    [Fact]
    public void ConfigureAuthorization_ShouldAddAuthorizationServices()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        AuthorizationConfiguration.ConfigureAuthorization(builder);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var authorizationOptions = serviceProvider.GetService<IOptions<AuthorizationOptions>>();
        Assert.NotNull(authorizationOptions);
        
        Assert.NotNull(authorizationOptions.Value); // Validate that AuthorizationOptions are present

        // Since DefaultPolicy is not null, you can check if it's an instance of AuthorizationPolicy
        Assert.IsType<AuthorizationPolicy>(authorizationOptions.Value.DefaultPolicy);

        // If you need to validate specific properties of DefaultPolicy, do so here
        var defaultPolicy = authorizationOptions.Value.DefaultPolicy;
        Assert.NotNull(defaultPolicy);
        // For example, check if it has any required claims, roles, or other properties
        // Assert.Contains("someClaim", defaultPolicy.Requirements);
    }
}