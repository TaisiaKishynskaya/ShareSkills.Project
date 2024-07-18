using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions;

public class StudentNotFoundException(Guid id)
    : NotFoundException($"The student with the identifier {id} was not found.")
{
    
}