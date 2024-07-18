using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IClassTimeRepository
{
    Task<ClassTimeEntity?> GetTeacherClassTimeAsync(string teacherClassTime);

    Task<ClassTimeEntity?> GetClassTimeAsync(Guid id);
}