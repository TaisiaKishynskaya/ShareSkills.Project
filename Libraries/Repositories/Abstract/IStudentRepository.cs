using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IStudentRepository
{
    Task<IEnumerable<StudentEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(StudentEntity student);
    void Remove(StudentEntity student);
}