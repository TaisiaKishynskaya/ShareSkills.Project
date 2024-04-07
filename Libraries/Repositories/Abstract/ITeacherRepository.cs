using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface ITeacherRepository
{
    Task<IEnumerable<TeacherEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TeacherEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(TeacherEntity teacher);
    void Remove(TeacherEntity teacher);
}