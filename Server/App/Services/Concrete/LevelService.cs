using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class LevelService : ILevelService
{
    private readonly ILevelRepository _levelRepository;
    private readonly ICacheService _cacheService;

    public LevelService(ILevelRepository levelRepository, ICacheService cacheService)
    {
        _levelRepository = levelRepository;
        _cacheService = cacheService;
    }

    public async Task AssignTeacherToLevelAsync(TeacherEntity teacher, string level)
    {
        var cacheKey = $"level:{level}";
        var cachedLevel = await _cacheService.GetCacheValueAsync<LevelEntity>(cacheKey);

        if (cachedLevel == null)
        {
            var levelEntity = await _levelRepository.GetTeacherLevelAsync(level)
                                ?? throw new Exception("Level doesn't exist");
            await _cacheService.SetCacheValueAsync(cacheKey, levelEntity);

            teacher.Level = levelEntity;
        }
        else
        {
            teacher.Level = cachedLevel;
        }
    }

    public async Task<string> GetLevelNameAsync(Guid id)
    {
        var cacheKey = $"level:{id}";
        var cachedLevel = await _cacheService.GetCacheValueAsync<LevelEntity>(cacheKey);

        if (cachedLevel == null)
        {
            var levelEntity = await _levelRepository.GetLevelAsync(id)
                                ?? throw new Exception("Level doesn't exist");
            
            await _cacheService.SetCacheValueAsync(cacheKey, levelEntity);

            return levelEntity.Name;
        } 
        
        return cachedLevel.Name;
    }
}
