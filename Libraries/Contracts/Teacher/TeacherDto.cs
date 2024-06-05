using Libraries.Contracts.Skill;
using Libraries.Entities.Concrete;

namespace Libraries.Contracts.Teacher;

public class TeacherDto
{
    public required Guid Id { get; set; }
    public required double Rating { get; set; }
    public required string ClassTime { get; set; }
    public required string Level { get; set; }
    public required string Skill { get; set; }
}