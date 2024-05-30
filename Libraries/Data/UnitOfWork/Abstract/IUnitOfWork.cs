using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Data.UnitOfWork.Abstract;

public interface  IUnitOfWork : IDisposable
{
    DbContext Context { get; }
    
    public IUserRepository UserRepository { get; }
    IStudentRepository StudentRepository { get; }
    ITeacherRepository TeacherRepository { get; }
    IMeetingRepository MeetingRepository { get; }
    IGradeRepository GradeRepository { get; }
    ISkillRepository SkillRepository { get; }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
