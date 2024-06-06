// using Libraries.Entities.Concrete;
//
// namespace Libraries.Repositories.Abstract;
//
// public interface ISkillRepository
// {
//     Task<IEnumerable<SkillEntity>> GetAllAsync(CancellationToken cancellationToken = default);
//     Task<SkillEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
//     void Insert(SkillEntity skill);
//     void Remove(SkillEntity skill);
// }

using Libraries.Entities.Concrete;
using System;
using System.Threading.Tasks;

namespace Libraries.Repositories.Abstract
{
    public interface ISkillRepository
    {
        Task<IEnumerable<SkillEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SkillEntity?> GetTeacherSkillAsync(string teacherSkill);
        Task<SkillEntity?> GetSkillAsync(Guid id);
    }
}
