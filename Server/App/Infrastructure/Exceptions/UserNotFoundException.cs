using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions;

public class UserNotFoundException(Guid id)
    : NotFoundException($"The user with the identifier {id} was not found.")
{
    
}