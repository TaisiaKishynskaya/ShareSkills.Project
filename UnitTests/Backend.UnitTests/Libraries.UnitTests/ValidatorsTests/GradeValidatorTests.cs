using FluentValidation.TestHelper;
using Libraries.Contracts.Grade;
using Libraries.Validators;

namespace Libraries.UnitTests.ValidatorsTests;

public class GradeValidatorTests
{
    private readonly GradeValidator _validator;

    public GradeValidatorTests()
    {
        _validator = new GradeValidator();
    }

    [Theory]
    [InlineData(1)]  // Valid value at the lower boundary
    [InlineData(3)]  // Valid value in the middle
    [InlineData(5)]  // Valid value at the upper boundary
    public void Validate_ShouldHaveNoErrors_WhenGradeIsValid(int grade)
    {
        // Arrange
        var dto = new GradeForCreatingDto { Grade = grade };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(g => g.Grade);
    }
    
    [Theory]
    [InlineData(0)]  // Invalid value below the lower boundary
    [InlineData(6)]  // Invalid value above the upper boundary
    public void Validate_ShouldHaveValidationError_WhenGradeIsInvalid(int grade)
    {
        // Arrange
        var dto = new GradeForCreatingDto { Grade = grade };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(g => g.Grade)
            .WithErrorMessage("Grade must be from 1 to 5");
    }
}