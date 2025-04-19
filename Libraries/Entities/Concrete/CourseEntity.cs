namespace Libraries.Entities.Concrete;

public class CourseEntity
{
    public required Guid Id { get; set; }

    public string Name { get; set; }
    
    // *-*
    public ICollection<TeacherEntity> Teachers { get; set; }
    
    // *-*
    public ICollection<UserEntity> Users { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}