namespace Libraries.Contracts.Teacher;

public class TeacherForUpdateDto
{
    public required double Rating { get; set; }
    public required string ClassTime { get; set; }
    public required string Level { get; set; }
    public required string Skill { get; set; }
}