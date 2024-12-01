using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class ClassTimeService : IClassTimeService
{
    private readonly IClassTimeRepository _classTimeRepository;
    private readonly ICacheService _cacheService;

    public ClassTimeService(IClassTimeRepository classTimeRepository, ICacheService cacheService)
    {
        _classTimeRepository = classTimeRepository;
        _cacheService = cacheService;
    }

    public async Task AssignTeacherToClassTimeAsync(TeacherEntity teacher, string classTime)
    {
        var cachedClassTime = await _cacheService.GetCacheValueAsync<ClassTimeEntity>($"classTime:{classTime}");

        if (cachedClassTime == null)
        {
            var classTimeEntity = await _classTimeRepository.GetTeacherClassTimeAsync(classTime)
                                  ?? throw new Exception("ClassTime doesn't exist");
            
            await _cacheService.SetCacheValueAsync($"classTime:{classTime}", classTimeEntity);

            teacher.ClassTime = classTimeEntity;
        }
        else
        {
            teacher.ClassTime = cachedClassTime;
        }
    }

    public async Task<string> GetClassTimeNameAsync(Guid id)
    {
        var cachedClassTime = await _cacheService.GetCacheValueAsync<ClassTimeEntity>($"classTime:{id}");

        if (cachedClassTime == null)
        {
            var classTimeEntity = await _classTimeRepository.GetClassTimeAsync(id)
                                  ?? throw new Exception("ClassTime doesn't exist");
            
            await _cacheService.SetCacheValueAsync($"classTime:{id}", classTimeEntity);

            return classTimeEntity.Name;
        }
        
        return cachedClassTime.Name;
    }
}
