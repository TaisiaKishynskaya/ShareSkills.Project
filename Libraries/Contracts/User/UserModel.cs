namespace Libraries.Contracts.User;

public class UserModel
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}