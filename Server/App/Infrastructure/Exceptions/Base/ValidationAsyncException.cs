namespace App.Infrastructure.Exceptions.Base;

public class ValidationAsyncException : Exception
{
    public ValidationAsyncException(string message) : base(message) { }
}