using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class LevelRepository(AppDbContext context) : ILevelRepository
{
    public Task<LevelEntity?> GetTeacherLevelAsync(string teacherLevel) => context.Levels.FirstOrDefaultAsync(x => x.Name == teacherLevel);
    public Task<LevelEntity?> GetLevelAsync(Guid id) => context.Levels.FirstOrDefaultAsync(x => x.Id == id);
}