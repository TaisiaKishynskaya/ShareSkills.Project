using System.Text.RegularExpressions;
using FluentValidation;
using Libraries.Contracts.User;

namespace Libraries.Validators;

public class UserValidator : AbstractValidator<UserForCreationDto>
{
    private readonly string _namesRestriction = @"^[A-Za-zА-Яа-яЁё-]+$";
    private readonly string _spaceRestriction = @"^\S+$";
    private readonly string _forgettingMessage = "You forgot about this field!";
    private readonly string _spaceRestrictionMessage = "This field must not contain spaces";
    private readonly string _namesRestrictionErrorMessage =
        "The 1st|2nd name can only contain letters (Latin or Cyrillic) and optionally a hyphen (-), spaces are not allowed.";
    
    public UserValidator()
    {
        RuleFor(n => n.Name)
            .NotEmpty().WithMessage(_forgettingMessage)
            .MaximumLength(20).WithMessage("Is your first name that long? Really??")
            .Matches(_namesRestriction).WithMessage(_namesRestrictionErrorMessage);

        RuleFor(n => n.Surname)
            .NotEmpty().WithMessage(_forgettingMessage)
            .Matches(_namesRestriction).WithMessage(_namesRestrictionErrorMessage);
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_forgettingMessage)
            .Matches(_spaceRestriction).WithMessage(_spaceRestrictionMessage)
            .EmailAddress().WithMessage("Incorrect format of email");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage(_forgettingMessage)
            .Matches(_spaceRestriction).WithMessage(_spaceRestrictionMessage)
            .Length(5, 15).WithMessage("Password must be at least 5 characters and maximum 15 characters")
            .Must(HasValidPassword).WithMessage("Password must contain lowercase, UPPERCASE, digits (1-9) and symbols"); // OR .Must(x => HasValidPassword(x));
    }

    private bool HasValidPassword(string pw)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");

        return lowercase.IsMatch(pw) && uppercase.IsMatch(pw) && digit.IsMatch(pw) && symbol.IsMatch(pw);
    }
}