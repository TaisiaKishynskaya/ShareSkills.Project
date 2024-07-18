namespace Libraries.Contracts.Meeting;

public class MeetingForCreatingDto
{
    public required string Name { get; set; }
    public required DateTime DateAndTime { get; set; }
    public required Guid OwnerId { get; set; }
    public required Guid ForeignId { get; set; }
}