using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IGradeRepository
{
    Task<IEnumerable<GradeEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GradeEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(GradeEntity grade);
    void Remove(GradeEntity grade);
}