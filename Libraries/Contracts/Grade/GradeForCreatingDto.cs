namespace Libraries.Contracts.Grade;

public class GradeForCreatingDto
{
    public Guid TeacherId { get; set; }
    public required int Grade { get; set; }
}