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

    public async Task<TeacherEntity?> GetByEmailAsync(string Email, CancellationToken cancellationToken = default)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == Email, cancellationToken);

            // Если пользователь не найден, возвращаем null
            if (user == null)
            {
                return null;
            }
            Console.WriteLine("id from repository:"+user.Id);

            // Находим учителя по найденному userId
            return await context.Teachers.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        }

    public async Task<IEnumerable<int>> GetScoresByTeacherIdAsync(Guid teacherId)
    {
        var grades = await context.Teachers
            .Where(t => t.Id == teacherId)
            .SelectMany(t => t.Grades)
            .ToListAsync();

        return grades.Select(x => x.Grade);
    }

    public void Insert(TeacherEntity teacher)
        => context.Teachers.Add(teacher);

    public void Remove(TeacherEntity teacher)
        => context.Teachers.Remove(teacher);
}