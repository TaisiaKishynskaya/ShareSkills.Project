namespace App.Infrastructure.Exceptions.Base;

public class FluentValidationException : Exception
{
    public FluentValidationException(string message) : base(message) { }
}