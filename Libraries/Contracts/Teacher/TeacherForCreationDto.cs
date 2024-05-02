namespace Libraries.Contracts.Teacher;

public class TeacherForCreationDto
{
    public required double Rating { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}