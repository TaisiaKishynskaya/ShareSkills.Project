using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface ISkillRepository
{
    Task<IEnumerable<SkillEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SkillEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(SkillEntity skill);
    void Remove(SkillEntity skill);
}