using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions.NotFoundExceptions;

public class GradeNotFoundException(Guid id)
    : NotFoundException($"The grade with the identifier {id} was not found.")
{
    
}