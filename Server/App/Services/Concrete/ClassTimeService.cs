using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class ClassTimeService(IClassTimeRepository classTimeRepository) : IClassTimeService
{
    public async Task AssignTeacherToClassTimeAsync(TeacherEntity teacher, string classTime)
    {
        teacher.ClassTime = await classTimeRepository.GetTeacherClassTimeAsync(classTime) ?? throw new Exception("ClassTime doesn't exists");
    }

    public async Task<string> GetClassTimeNameAsync(Guid id)
    {
        var classTime = await classTimeRepository.GetClassTimeAsync(id) ?? throw new Exception("ClassTime doesn't exists");
        return classTime.Name;
    }
}