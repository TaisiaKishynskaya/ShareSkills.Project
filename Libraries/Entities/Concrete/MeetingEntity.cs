// using Org.Apache.Http.Protocol;


namespace Libraries.Entities.Concrete;

public class MeetingEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DateTime { get; set; }
    public string? Description { get; set; }
    public required Guid OwnerId { get; set; }
    public required Guid ForeignId { get; set; }
}