using Libraries.Contracts.Skill;

namespace Libraries.Contracts.Teacher;

public class TeacherDto
{
    public required Guid Id { get; set; }
    public required double Rating { get; set; }
    public required string TimeOfDay { get; set; }
    public required string Goal { get; set; }
    public required SkillDto Skill { get; set; }
}