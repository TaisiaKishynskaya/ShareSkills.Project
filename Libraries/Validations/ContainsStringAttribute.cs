using System.ComponentModel.DataAnnotations;

namespace Libraries.Validations;

public class ContainsStringAttribute : ValidationAttribute
{
    private readonly string _substring;

    public ContainsStringAttribute(string substring)
    {
        _substring = substring;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null && value is string str && str.Contains(_substring))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult($"Поле має містити строку: {_substring}");
    }
}