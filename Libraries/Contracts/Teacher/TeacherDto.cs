using Libraries.Contracts.Skill;

namespace Libraries.Contracts.Teacher;

public class TeacherDto
{
    public required Guid Id { get; set; }
    public required double Rating { get; set; }
    public required string ClassTime { get; set; }
    public required string Level { get; set; }
    public required SkillDto Skill { get; set; }
}