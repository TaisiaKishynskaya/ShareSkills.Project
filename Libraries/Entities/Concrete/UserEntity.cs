using System.ComponentModel.DataAnnotations;
using Libraries.Validations;

namespace Libraries.Entities.Concrete;

public class UserEntity
{
    public required Guid Id { get; set; }
    [Required(ErrorMessage = "Cannot be empty!")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name should contain only letters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Cannot be empty!")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Surname should contain only letters")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Cannot be empty!")]
    [EmailAddress(ErrorMessage = "Incorrect Email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Cannot be empty!")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
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
