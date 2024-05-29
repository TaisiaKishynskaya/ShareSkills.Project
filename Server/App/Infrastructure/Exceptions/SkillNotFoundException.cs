using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions;

public class SkillNotFoundException(Guid id)
    : NotFoundException($"The skill with the identifier {id} was not found.")
{
    
}