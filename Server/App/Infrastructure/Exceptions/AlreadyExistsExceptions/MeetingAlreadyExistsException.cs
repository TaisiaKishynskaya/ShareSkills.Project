namespace App.Infrastructure.Exceptions.AlreadyExistsExceptions;

public class MeetingAlreadyExistsException(string message) : Exception(message);