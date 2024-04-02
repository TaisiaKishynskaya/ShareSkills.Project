namespace Libraries.Entities.Concrete;

public class MeetingEntity
{
    public int Id { get; set; }
    public DateTime DateAndTime { get; set; }
    
    // 1-*
    public int TeacherId { get; set; }
    public TeacherEntity Teacher { get; set; }
    
    // 1-*
    public int StudentId { get; set; }
    public StudentEntity Student { get; set; }
}