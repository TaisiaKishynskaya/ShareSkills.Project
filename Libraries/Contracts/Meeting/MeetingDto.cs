namespace Libraries.Contracts.Meeting;

public class MeetingDto
{
    public required Guid Id { get; set; }
    public required DateTime DateTime { get; set; }
    public string? Description { get; set; }
    public required Guid OwnerId { get; set; }
    public required Guid ForeignId { get; set; }
}