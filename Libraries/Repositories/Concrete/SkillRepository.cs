// using Libraries.Data;
// using Libraries.Entities.Concrete;
// using Libraries.Repositories.Abstract;
// using Microsoft.EntityFrameworkCore;
//
// namespace Libraries.Repositories.Concrete;
//
// public class SkillRepository (AppDbContext context) : ISkillRepository
// {
//     public async Task<IEnumerable<SkillEntity>> GetAllAsync(CancellationToken cancellationToken = default)
//         => await context.Skills.ToListAsync(cancellationToken);
//
//     public async Task<SkillEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//         => await context.Skills.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
//
//     public void Insert(SkillEntity skill)
//         => context.Skills.Add(skill);
//
//     public void Remove(SkillEntity skill)
//         => context.Skills.Remove(skill);
// }

using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Libraries.Repositories.Concrete
{
    public class SkillRepository(AppDbContext context) : ISkillRepository
    {
        public Task<SkillEntity?> GetTeacherSkillAsync(string teacherSkill) => context.Skills.FirstOrDefaultAsync(x => x.Skill == teacherSkill);
        public Task<SkillEntity?> GetSkillAsync(Guid id) => context.Skills.FirstOrDefaultAsync(x => x.Id == id);
    }
}
