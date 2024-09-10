using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions.NotFoundExceptions;

public class SkillNotFoundException(Guid id)
    : NotFoundException($"The skill with the identifier {id} was not found.")
{
    
}