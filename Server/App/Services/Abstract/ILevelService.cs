using Libraries.Entities.Concrete;

namespace App.Services.Abstract;

public interface ILevelService
{
    Task AssignTeacherToLevelAsync(TeacherEntity teacher, string level);
    
    Task<string> GetLevelNameAsync(Guid id);
}