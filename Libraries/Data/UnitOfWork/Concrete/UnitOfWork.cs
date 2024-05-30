using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Repositories.Abstract;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Data.UnitOfWork.Concrete;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private bool _disposed = false;

    public DbContext Context => context;
    
    public IUserRepository UserRepository { get; } = new UserRepository(context);
    public IStudentRepository StudentRepository { get; } = new StudentRepository(context);
    public ITeacherRepository TeacherRepository { get; } = new TeacherRepository(context);
    public IMeetingRepository MeetingRepository { get; } = new MeetingRepository(context);
    public IGradeRepository GradeRepository { get; } = new GradeRepository(context);
    public ISkillRepository SkillRepository { get; } = new SkillRepository(context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }

            _disposed = true;
        }
    }
}
