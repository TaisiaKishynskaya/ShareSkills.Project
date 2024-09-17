using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.Abstract;
using FluentValidation;
using FluentValidation.Results;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Server.UnitTests.EndpointsTests;

public class AuthEndpointsTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IValidator<UserForCreationDto>> _validatorMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public AuthEndpointsTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _validatorMock = new Mock<IValidator<UserForCreationDto>>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Настраиваем конфигурацию JWT
        _configurationMock.SetupGet(c => c["Jwt:Issuer"]).Returns("testIssuer");
        _configurationMock.SetupGet(c => c["Jwt:Audience"]).Returns("testAudience");
        _configurationMock.SetupGet(c => c["Jwt:Key"]).Returns("testKey12345678901234567890");
    }

    [Fact]
    public async Task RegisterUser_ReturnsOkResultWithUserId_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var userForCreation = new UserForCreationDto
        {
            Name = "Test",
            Surname = "User",
            Email = "test@example.com",
            Password = "Password123",
            Role = "User"
        };

        var validationResult = new ValidationResult();
        _validatorMock.Setup(v => v.ValidateAsync(userForCreation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _userServiceMock.Setup(s => s.GetByEmailAsync(userForCreation.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto)null); // Пользователь не найден

        var expectedUserId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        var cancellationToken = new CancellationToken();

        // Эмулируем делегат, созданный в MapPost
        var endpointDelegate = async (IUserService userService, UserForCreationDto user, IValidator<UserForCreationDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            if (await userService.GetByEmailAsync(user.Email) is not null)
            {
                return Results.BadRequest("User already exists");
            }

            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role
            };

            var passwordHasher = new PasswordHasher<UserModel>();
            var passwordHash = passwordHasher.HashPassword(userModel, user.Password);
            userModel.PasswordHash = passwordHash;

            var userId = await userService.CreateAsync(userModel);

            return Results.Ok(userId);
        };

        // Act
        var result = await endpointDelegate(_userServiceMock.Object, userForCreation, _validatorMock.Object);

        // Assert
        var okResult = Assert.IsType<Ok<Guid>>(result);
        Assert.Equal(expectedUserId, okResult.Value);
    }
    
    [Fact]
    public async Task RegisterUser_ReturnsBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var userForCreation = new UserForCreationDto
        {
            Name = "Test",
            Surname = "User",
            Email = "test@example.com",
            Password = "Password123",
            Role = "User"
        };

        var validationResult = new ValidationResult();
        _validatorMock.Setup(v => v.ValidateAsync(userForCreation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _userServiceMock.Setup(s => s.GetByEmailAsync(userForCreation.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto { Id = default, PasswordHash = null, Name = null, Surname = null, Email = null }); // Пользователь уже существует

        var cancellationToken = new CancellationToken();

        // Эмулируем делегат, созданный в MapPost
        var endpointDelegate = async (IUserService userService, UserForCreationDto user, IValidator<UserForCreationDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)  return Results.ValidationProblem(validationResult.ToDictionary());

            return await userService.GetByEmailAsync(user.Email) is not null ? Results.BadRequest("User already exists") : Results.Ok(Guid.NewGuid());
        };

        // Act
        var result = await endpointDelegate(_userServiceMock.Object, userForCreation, _validatorMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("User already exists", badRequestResult.Value);
    }
    
    [Fact]
    public async Task RegisterUser_ReturnsValidationProblem_WhenValidationFails()
    {
        // Arrange
        var userForCreation = new UserForCreationDto
        {
            Name = "Test",
            Surname = "User",
            Email = "test@example.com",
            Password = "Password123",
            Role = "User"
        };

        var validationResult = new ValidationResult(new List<ValidationFailure> { new("Email", "Invalid email address") });

        _validatorMock.Setup(v => v.ValidateAsync(userForCreation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var cancellationToken = new CancellationToken();

        // Эмулируем делегат, созданный в MapPost
        var endpointDelegate = async (IUserService userService, UserForCreationDto user, IValidator<UserForCreationDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid) return Results.ValidationProblem(validationResult.ToDictionary());

            return await userService.GetByEmailAsync(user.Email) is not null ? Results.BadRequest("User already exists") : Results.Ok(Guid.NewGuid());
        };

        // Создаем HttpContext с сервисами
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = new ServiceCollection()
            .AddLogging() // Добавляем нужные сервисы
            .BuildServiceProvider();
        
        httpContext.Response.Body = new MemoryStream(); // Включаем запись в Response.Body

        // Act
        var result = await endpointDelegate(_userServiceMock.Object, userForCreation, _validatorMock.Object);
        
        await result.ExecuteAsync(httpContext); // Выполняем результат
        
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin); // Читаем статус ответа
        
        Assert.Equal(400, httpContext.Response.StatusCode); // Проверяем статус ответа (ожидаем статус 400 для ValidationProblem)
        
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();  // Читаем ответное тело

        // Десериализуем ValidationProblemDetails
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal("Invalid email address", problemDetails.Errors["Email"].First());
    }

    
    [Fact]
    public async Task Login_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123";

        _userServiceMock.Setup(s => s.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto)null);

        var endpointDelegate = async (IUserService userService, IConfiguration config, string email, string password) =>
        {
            var user = await userService.GetByEmailAsync(email);

            if (user is null) return Results.NotFound("User not found");

            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            var passwordHasher = new PasswordHasher<UserModel>();
            if (passwordHasher.VerifyHashedPassword(userModel, userModel.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return Results.BadRequest("Wrong password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken
            (
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new { Token = tokenString, UserId = user.Id });
        };

        // Act
        var result = await endpointDelegate(_userServiceMock.Object, _configurationMock.Object, email, password);

        // Assert
        var notFoundResult = Assert.IsType<NotFound<string>>(result);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var email = "test@example.com";
        var wrongPassword = "WrongPassword";
        var correctPassword = "CorrectPassword"; // Simulated correct password
        var userId = Guid.NewGuid();

        var userDto = new UserDto
        {
            Id = userId,
            Name = "John",
            Surname = "Doe",
            Email = email,
            PasswordHash = HashPassword(correctPassword), // Provide a valid hash for correct password
            Role = "User"
        };

        _userServiceMock.Setup(s => s.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDto);

        var config = new Mock<IConfiguration>();
        config.SetupGet(c => c["Jwt:Issuer"]).Returns("issuer");
        config.SetupGet(c => c["Jwt:Audience"]).Returns("audience");
        config.SetupGet(c => c["Jwt:Key"]).Returns("secret");

        var endpointDelegate = async (IUserService userService, IConfiguration config, string email, string password) =>
        {
            var user = await userService.GetByEmailAsync(email);

            if (user is null) return Results.NotFound("User not found");

            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            var passwordHasher = new PasswordHasher<UserModel>();
            if (passwordHasher.VerifyHashedPassword(userModel, userModel.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return Results.BadRequest("Wrong password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken
            (
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new { Token = tokenString, UserId = user.Id });
        };

        // Act
        var result = await endpointDelegate(_userServiceMock.Object, config.Object, email, wrongPassword);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Wrong password", badRequestResult.Value);
    }

    private string HashPassword(string password)
    {
        var hasher = new PasswordHasher<UserModel>();
        var userModel = new UserModel
        {
            PasswordHash = hasher.HashPassword(null, password), Name = null, Surname = null, Email = null, Role = null
        };
        return userModel.PasswordHash;
    }
}