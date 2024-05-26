using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class TeacherRepository(AppDbContext context) : ITeacherRepository
{
    public async Task<IEnumerable<TeacherEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Teachers.ToListAsync(cancellationToken);

    public async Task<TeacherEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Teachers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Insert(TeacherEntity teacher)
        => context.Teachers.Add(teacher);

    public void Remove(TeacherEntity teacher)
        => context.Teachers.Remove(teacher);
}