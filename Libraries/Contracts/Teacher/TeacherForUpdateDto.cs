namespace Libraries.Contracts.Teacher;

public class TeacherForUpdateDto
{
    public required double Rating { get; set; }
    public required string TimeOfDay { get; set; }
    public required string Goal { get; set; }
    public required string Skill { get; set; }
}