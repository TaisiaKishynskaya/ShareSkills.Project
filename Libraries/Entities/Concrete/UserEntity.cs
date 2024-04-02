namespace Libraries.Entities.Concrete;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Objective { get; set; }
    public string Password { get; set; }
    
    // 1-1
    public StudentEntity Student { get; set; }
    
    // 1-1
    public TeacherEntity Teacher { get; set; }
    
    // *-*
    public ICollection<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
}