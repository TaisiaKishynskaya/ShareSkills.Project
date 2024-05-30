namespace App.Infrastructure.Exceptions;

public class MeetingAlreadyExistsException(string message) : Exception(message);