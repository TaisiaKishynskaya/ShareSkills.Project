using FluentValidation.TestHelper;
using Libraries.Contracts.User;
using Libraries.Validators;

namespace Libraries.UnitTests.ValidatorsTests;

public class UserValidatorTests
{
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _validator = new UserValidator();
    }
    
    [Theory]
    [InlineData("", "Surname", "email@example.com", "Password1@", "You forgot about this field!")]
    [InlineData("John", "", "email@example.com", "Password1@", "You forgot about this field!")]
    [InlineData("John", "Doe", "", "Password1@", "You forgot about this field!")]
    [InlineData("John", "Doe", "email@example.com", "", "You forgot about this field!")]
    public void Should_Have_Error_When_Fields_Are_Empty(string name, string surname, string email, string password, string expectedErrorMessage)
    {
        // Arrange
        var dto = new UserForCreationDto
        {
            Name = name,
            Surname = surname,
            Email = email,
            Password = password,
            Role = "Student"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (string.IsNullOrEmpty(name))
        {
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(expectedErrorMessage);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        if (string.IsNullOrEmpty(surname))
        {
            result.ShouldHaveValidationErrorFor(x => x.Surname)
                .WithErrorMessage(expectedErrorMessage);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Surname);
        }

        if (string.IsNullOrEmpty(email))
        {
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(expectedErrorMessage);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        if (string.IsNullOrEmpty(password))
        {
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(expectedErrorMessage);
        }
        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
    
    [Theory]
    [InlineData("John", "Doe", "email@example.com", "Password1@", null)] // Valid input, no expected error
    [InlineData("JohnDoeJohnDoeJohnDoe", "Doe", "email@example.com", "Password1@", "Is your first name that long? Really??")] // Name too long
    [InlineData("John", "Doe Doe", "email@example.com", "Password1@", "This field must not contain spaces")] // Surname with space
    [InlineData("John", "Doe", "email @example.com", "Password1@", "This field must not contain spaces")] // Email with space
    [InlineData("John", "Doe", "email@example.com", "pass", "Password must be at least 5 characters and maximum 15 characters")] // Password too short
    [InlineData("John", "Doe", "email@example.com", "passwordwithoutdigits", "Password must contain lowercase, UPPERCASE, digits (1-9) and symbols")] // Password missing digits/uppercase
    public void Should_Validate_UserDto_Correctly(string name, string surname, string email, string password, string expectedErrorMessage)
    {
        // Arrange
        var dto = new UserForCreationDto
        {
            Name = name,
            Surname = surname,
            Email = email,
            Password = password,
            Role = "Teacher"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        if (string.IsNullOrEmpty(expectedErrorMessage))
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            // If an error message is expected, assert that the appropriate field has the expected error message
            if (expectedErrorMessage.Contains("name")) // Assuming name should be checked
                result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedErrorMessage);
            
            if (expectedErrorMessage.Contains("surname")) // Assuming surname should be checked
                result.ShouldHaveValidationErrorFor(x => x.Surname).WithErrorMessage(expectedErrorMessage);
            
            if (expectedErrorMessage.Contains("email")) // Assuming email should be checked
                result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(expectedErrorMessage);
            
            if (expectedErrorMessage.Contains("password")) // Assuming password should be checked
                result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(expectedErrorMessage);
        }
    }
}