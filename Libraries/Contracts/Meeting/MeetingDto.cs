namespace Libraries.Contracts.Meeting;

public class MeetingDto
{
    public required Guid Id { get; set; }
    public required DateTime DateAndTime { get; set; }
}