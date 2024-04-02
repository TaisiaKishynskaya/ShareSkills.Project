namespace Libraries.Entities.Concrete;

public class GradeEntity
{
    public int Id { get; set; }
    public int Grade { get; set; }
    
    // *-*
    public ICollection<TeacherEntity> Teachers { get; set; } = new List<TeacherEntity>();
    
    // *-*
    public ICollection<StudentEntity> Students { get; set; } = new List<StudentEntity>();
}