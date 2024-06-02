using Libraries.Contracts.Skill;
using Libraries.Entities.Concrete;

namespace Libraries.Contracts.Teacher;

public class TeacherForCreationDto
{
    public required double Rating { get; set; }
    public required ClassTimeEntity ClassTime { get; set; }
    public required LevelEntity Level { get; set; }
    public required SkillDto Skill { get; set; }
}