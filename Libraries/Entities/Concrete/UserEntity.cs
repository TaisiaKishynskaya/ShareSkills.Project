using System.ComponentModel;

namespace Libraries.Entities.Concrete;

public class UserEntity
{
    public required Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    
    [PasswordPropertyText]
    public string Password { get; set; }
    
    // *-1
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;
    
    // 1-1
    public StudentEntity Student { get; set; }
    
    // 1-1
    public TeacherEntity Teacher { get; set; }
    
    // *-*
    public ICollection<MeetingEntity> Meetings { get; set; } = new List<MeetingEntity>();
    // *-*
    public ICollection<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
}