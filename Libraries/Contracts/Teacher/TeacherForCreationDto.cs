using Libraries.Contracts.Skill;
using Libraries.Contracts.Level;
using Libraries.Contracts.ClassTime;
using Libraries.Entities.Concrete;

namespace Libraries.Contracts.Teacher;

public class TeacherForCreationDto
{
    public required Guid UserId { get; set; }
    public required double Rating { get; set; }
    public required Guid ClassTime { get; set; }
    public required Guid Level { get; set; }
    public required Guid Skill { get; set; }
}