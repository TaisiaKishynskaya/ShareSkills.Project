using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Libraries.Validations
{
    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string email)
            {
                if (Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Missing @.");
                }
            }

            return new ValidationResult("Missing @");
        }
    }
}