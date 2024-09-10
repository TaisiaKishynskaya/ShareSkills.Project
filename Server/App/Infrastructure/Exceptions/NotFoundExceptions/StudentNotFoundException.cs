using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions.NotFoundExceptions;

public class StudentNotFoundException(Guid id)
    : NotFoundException($"The student with the identifier {id} was not found.")
{
    
}