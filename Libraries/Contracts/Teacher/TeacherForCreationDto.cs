using Libraries.Contracts.Skill;
using Libraries.Contracts.Level;
using Libraries.Contracts.ClassTime;
using Libraries.Entities.Concrete;

namespace Libraries.Contracts.Teacher;

public class TeacherForCreationDto
{
    public required double Rating { get; set; }
    public required string ClassTime { get; set; }
    public required string Level { get; set; }
    public required SkillEntity Skill { get; set; }
}