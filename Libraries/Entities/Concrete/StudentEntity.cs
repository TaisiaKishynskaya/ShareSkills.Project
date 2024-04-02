namespace Libraries.Entities.Concrete;

public class StudentEntity
{
    public int Id { get; set; }
    public string Purpose { get; set; }
    
    // 1-1
    public int UserId { get; set; }
    public required UserEntity User { get; set; }
    
    // 1-*
    public ICollection<MeetingEntity>? Meetings { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}