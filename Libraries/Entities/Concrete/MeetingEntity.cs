namespace Libraries.Entities.Concrete;

public class MeetingEntity
{
    public required Guid Id { get; set; }
    public DateTime DateAndTime { get; set; }
    
    // 1-*
    public Guid TeacherId { get; set; }
    public TeacherEntity Teacher { get; set; }
    
    // 1-*
    public Guid StudentId { get; set; }
    public StudentEntity Student { get; set; }
}