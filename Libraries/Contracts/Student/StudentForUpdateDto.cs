namespace Libraries.Contracts.Student;

public class StudentForUpdateDto
{
    public string Purpose { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}