using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class ClassTimeRepository(AppDbContext context) : IClassTimeRepository
{
    public Task<ClassTimeEntity?> GetTeacherClassTimeAsync(string teacherClassTime) => context.ClassTimes.FirstOrDefaultAsync(x => x.Name == teacherClassTime);
    public Task<ClassTimeEntity?> GetClassTimeAsync(Guid id) => context.ClassTimes.FirstOrDefaultAsync(x => x.Id == id);
}