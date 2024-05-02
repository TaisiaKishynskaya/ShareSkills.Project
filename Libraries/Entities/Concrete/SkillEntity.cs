namespace Libraries.Entities.Concrete;

public class SkillEntity
{
    public required Guid Id { get; set; }
    public string Skill { get; set; }
    
    // *-*
    // public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();

    public ICollection<StudentEntity> Students { get; set; } = new List<StudentEntity>();
    public ICollection<TeacherEntity> Teachers { get; set; } = new List<TeacherEntity>();
}