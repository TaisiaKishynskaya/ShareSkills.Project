namespace Libraries.Entities.Concrete;

public class TeacherEntity
{
    public required Guid Id { get; set; }
    public double Rating { get; set; }
    
    // 1-1
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}