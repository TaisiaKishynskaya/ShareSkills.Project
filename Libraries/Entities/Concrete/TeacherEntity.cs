using Libraries.Contracts.Skill;

namespace Libraries.Entities.Concrete;

public class TeacherEntity
{
    public required Guid Id { get; set; }
    public double Rating { get; set; }
    
    // *-1
    public Guid TimeOfDayId { get; set; }
    public TimeOfDayEntity TimeOfDay { get; set; } = null!;
    
    public Guid GoalId { get; set; }
    public GoalEntity Goal { get; set; } = null!;
    
    public Guid SkillId { get; set; }
    public SkillDto Skill { get; set; } = null!;
 
    // 1-1
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    
    // *-*
    public ICollection<GradeEntity> Grades { get; set; } = new List<GradeEntity>();
}