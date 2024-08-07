namespace Libraries.Entities.Concrete;

public class StudentEntity
{
    public required Guid Id { get; set; }
    public string Purpose { get; set; }
    
    // 1-1
    public required Guid UserId { get; set; }
    public UserEntity User { get; set; }
    
    // 1-*
    public ICollection<MeetingEntity>? Meetings { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}