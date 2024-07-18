using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class LevelService(ILevelRepository levelRepository) : ILevelService
{
    public async Task AssignTeacherToLevelAsync(TeacherEntity teacher, string level)
    {
        teacher.Level = await levelRepository.GetTeacherLevelAsync(level) ?? throw new Exception("Level doesn't exists");
    }

    public async Task<string> GetLevelNameAsync(Guid id)
    {
        var level = await levelRepository.GetLevelAsync(id) ?? throw new Exception("Level doesn't exists");
        return level.Name;
    }
}