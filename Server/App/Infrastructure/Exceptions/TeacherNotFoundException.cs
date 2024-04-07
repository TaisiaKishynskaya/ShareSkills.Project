using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions;

public class TeacherNotFoundException(Guid id)
    : NotFoundException($"The teacher with the identifier {id} was not found.")
{
    
}