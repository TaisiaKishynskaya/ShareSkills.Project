using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class StudentRepository(AppDbContext context) : IStudentRepository
{
    public async Task<IEnumerable<StudentEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Students.ToListAsync(cancellationToken);

    public async Task<StudentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Insert(StudentEntity student)
        => context.Students.Add(student);

    public void Remove(StudentEntity student)
        => context.Students.Remove(student);
}