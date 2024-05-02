// namespace Libraries.Contracts.Teacher;
//
// public class TeacherDto
// {
//     public required Guid Id { get; set; }
//     public required double Rating { get; set; }
// }

namespace Libraries.Contracts.Teacher;

public class TeacherDto
{
    private Guid _id;
    public required Guid Id
    {
        get { return _id; }
        set
        {
            if (Guid.TryParse(value.ToString(), out Guid result))
            {
                _id = result;
            }
            else
            {
                throw new ArgumentException("Invalid GUID format");
            }
        }
    }
    public required double Rating { get; set; }
    
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}