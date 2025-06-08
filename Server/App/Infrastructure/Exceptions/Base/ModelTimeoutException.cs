namespace App.Infrastructure.Exceptions.Base;

public class ModelTimeoutException : Exception
{
    public ModelTimeoutException(string message)
        : base(message)
    {
    }
}
