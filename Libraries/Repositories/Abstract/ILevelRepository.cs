using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface ILevelRepository
{
    Task<LevelEntity?> GetTeacherLevelAsync(string teacherLevel);

    Task<LevelEntity?> GetLevelAsync(Guid id);
}