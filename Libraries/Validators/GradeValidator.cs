using FluentValidation;
using Libraries.Contracts.Grade;

namespace Libraries.Validators;

public class GradeValidator : AbstractValidator<GradeForCreatingDto>
{
    public GradeValidator()
    {
        RuleFor(g => g.Grade)
            .InclusiveBetween(1, 5).WithMessage("Grade must be from 1 to 5");
    }
}