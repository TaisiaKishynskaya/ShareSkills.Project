namespace Libraries.Contracts.User;

public class UserForCreationDto
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}