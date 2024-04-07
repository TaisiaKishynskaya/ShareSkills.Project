using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Data.UnitOfWork.Abstract;

public interface  IUnitOfWork : IDisposable
{
    DbContext Context { get; }
    
    IStudentRepository StudentRepository { get; }
    ITeacherRepository TeacherRepository { get; }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
