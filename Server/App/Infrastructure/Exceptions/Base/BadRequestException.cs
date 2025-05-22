namespace App.Infrastructure.Exceptions.Base;

public class BadRequestException : ApplicationException
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}