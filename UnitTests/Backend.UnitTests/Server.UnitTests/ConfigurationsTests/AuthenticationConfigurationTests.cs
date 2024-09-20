using System.Text;
using App.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Server.UnitTests.ConfigurationsTests;

public class AuthenticationConfigurationTests
{
    [Fact]
    public void ConfigureAuthentication_ShouldConfigureJwtBearerAuthentication()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["Jwt:Issuer"] = "yourIssuer";
        builder.Configuration["Jwt:Audience"] = "yourAudience";
        builder.Configuration["Jwt:Key"] = "yourSuperSecretKey";

        // Act
        AuthenticationConfiguration.ConfigureAuthentication(builder);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var jwtBearerOptions = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.NotNull(jwtBearerOptions);
        Assert.Equal("yourIssuer", jwtBearerOptions.TokenValidationParameters.ValidIssuer);
        Assert.Equal("yourAudience", jwtBearerOptions.TokenValidationParameters.ValidAudience);
        Assert.Equal("yourSuperSecretKey", Encoding.UTF8.GetString(((SymmetricSecurityKey)jwtBearerOptions.TokenValidationParameters.IssuerSigningKey).Key));
    }
}