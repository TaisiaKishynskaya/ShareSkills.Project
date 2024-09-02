using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions.NotFoundExceptions;

public class TeacherNotFoundException(Guid id)
    : NotFoundException($"The teacher with the identifier {id} was not found.")
{
    
}