namespace App.Infrastructure.Exceptions.Base;

public class NotFoundException(string message) : Exception(message)
{
    
}