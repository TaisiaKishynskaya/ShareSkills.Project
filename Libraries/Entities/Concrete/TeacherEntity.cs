using Libraries.Contracts.ClassTime;
using Libraries.Contracts.Level;
using Libraries.Contracts.Skill;

namespace Libraries.Entities.Concrete;

public class TeacherEntity
{
    public required Guid Id { get; set; }
    public double Rating { get; set; }
    
    // *-1
    public Guid ClassTimeId { get; set; }
    public ClassTimeEntity ClassTime { get; set; } = null!;
    
    public Guid LevelId { get; set; }
    public LevelEntity Level { get; set; } = null!;
    
    public Guid SkillId { get; set; }
    public SkillEntity Skill { get; set; } = null!;
 
    // 1-1
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}