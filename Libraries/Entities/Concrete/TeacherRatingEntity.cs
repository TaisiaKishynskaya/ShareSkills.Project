namespace Libraries.Entities.Concrete;

public class TeacherRatingEntity
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid TeacherId { get; set; }
    public int Rating { get; set; }
    
    public StudentEntity Student { get; set; }
    public TeacherEntity Teacher { get; set; }
}