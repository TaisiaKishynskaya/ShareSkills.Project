using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Libraries.Repositories.Concrete;

public class GradeRepository (AppDbContext context) : IGradeRepository
{
    public async Task<IEnumerable<GradeEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Grades.ToListAsync(cancellationToken);

    public async Task<GradeEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Grades.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Insert(GradeEntity grade)
        => context.Grades.Add(grade);

    public void Remove(GradeEntity grade)
        => context.Grades.Remove(grade);
}