namespace Libraries.Entities.Concrete;

public class UserEntity
{
    public required Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!; 
    //public string Objective { get; set; }
    
    // 1-1
    //public Guid StudentId { get; set; }
    public StudentEntity Student { get; set; }
    
    // 1-1
    //public Guid TeacherId { get; set; }
    public TeacherEntity Teacher { get; set; }
    
    // *-*
    public ICollection<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
}