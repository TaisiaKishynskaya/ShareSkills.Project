using App.Infrastructure.Exceptions.Base;

namespace App.Infrastructure.Exceptions;

public class MeetingNotFoundException(Guid id)
    : NotFoundException($"The meeting with the identifier {id} was not found.")
{
    
}