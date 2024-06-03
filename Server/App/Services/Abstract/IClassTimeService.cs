using Libraries.Entities.Concrete;

namespace App.Services.Abstract;

public interface IClassTimeService
{
    Task AssignTeacherToClassTimeAsync(TeacherEntity teacher, string classTime);
    
    Task<string> GetClassTimeNameAsync(Guid id);
}