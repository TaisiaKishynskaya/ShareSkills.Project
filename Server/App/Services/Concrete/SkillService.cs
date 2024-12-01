// using App.Infrastructure.Exceptions;
// using App.Services.Abstract;
// using Libraries.Contracts.Skill;
// using Libraries.Data.UnitOfWork.Abstract;
// using Libraries.Entities.Concrete;
// using Libraries.Repositories.Abstract;
//
// namespace App.Services.Concrete;
//
// public class SkillService (IUnitOfWork unitOfWork, ISkillRepository skillRepository) : ISkillService
// {
//     public async Task<SkillDto> CreateAsync(SkillForCreatingDto skillForCreatingDto, CancellationToken cancellationToken = default)
//     {
//         var skill = new SkillEntity
//         {
//             Id = Guid.NewGuid(),
//             Skill = skillForCreatingDto.Skill,
//         };
//
//         unitOfWork.SkillRepository.Insert(skill);
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//
//         return new SkillDto
//         {
//             Id = skill.Id,
//             Skill = skill.Skill
//         };
//     }
//
//     public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         var skill = await unitOfWork.SkillRepository
//                         .GetByIdAsync(id, cancellationToken)
//                     ?? throw new SkillNotFoundException(id);
//
//         unitOfWork.SkillRepository.Remove(skill);
//
//         //await userService.DeleteAsync(_userDto.Id, cancellationToken);
//
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//     }
//
//     public async Task<IEnumerable<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default)
//     {
//         var skills = await unitOfWork.SkillRepository
//             .GetAllAsync(cancellationToken);
//
//         var skillsDtos = new List<SkillDto>();
//
//         foreach (var skill in skills)
//         {
//             skillsDtos.Add(new SkillDto
//             {
//                 Id = skill.Id,
//                 Skill = skill.Skill,
//             });
//         }
//
//         return skillsDtos;
//     }
//
//     public async Task<SkillDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         var skill = await unitOfWork.SkillRepository
//                         .GetByIdAsync(id, cancellationToken)
//                     ?? throw new SkillNotFoundException(id);
//
//         return new SkillDto
//         {
//             Id = skill.Id,
//             Skill = skill.Skill
//         };
//     }
//
//     public async Task UpdateAsync(Guid id, SkillForUpdateDto skillForUpdateDto, CancellationToken cancellationToken = default)
//     {
//         var skill = await unitOfWork.SkillRepository
//                         .GetByIdAsync(id, cancellationToken)
//                     ?? throw new SkillNotFoundException(id);
//
//         skill.Skill = skillForUpdateDto.Skill;
//
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//     }
//     
//     // public async Task<string> GetSkillNameAsync(Guid id)
//     // {
//     //     var skill = await skillRepository.GetLevelAsync(id) ?? throw new Exception("Skill doesn't exists");
//     //     return skill.Name;
//     // }
// }

using App.Services.Abstract;
using Libraries.Contracts.Skill;
using Libraries.Repositories.Abstract;
using System.Text.Json;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;
    private readonly ICacheService _cacheService;

    public SkillService(ISkillRepository skillRepository, ICacheService cacheService)
    {
        _skillRepository = skillRepository;
        _cacheService = cacheService;
    }

    public async Task AssignTeacherToSkillAsync(TeacherEntity teacher, string skill)
    {
        teacher.Skill = await _skillRepository.GetTeacherSkillAsync(skill) ??
                        throw new Exception("Skill doesn't exist");
    }

    public async Task<string> GetSkillNameAsync(Guid id)
    {
        var cacheKey = $"skill:{id}";
        
        var cachedSkill = await _cacheService.GetCacheValueAsync<string>(cacheKey);
        if (cachedSkill != null)
        {
            return cachedSkill;
        }
        
        var skill = await _skillRepository.GetSkillAsync(id) ?? throw new Exception("Skill doesn't exist");
        
        await _cacheService.SetCacheValueAsync(cacheKey, skill.Skill);

        return skill.Skill;
    }

    public async Task<IEnumerable<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "skills";
        
        var cachedSkills = await _cacheService.GetCacheValueAsync<List<SkillDto>>(cacheKey);

        if (cachedSkills != null)
        {
            return cachedSkills;
        }

        var skills = await _skillRepository.GetAllAsync(cancellationToken);
        
        var skillDtos = skills.Select(skill => new SkillDto
        {
            Id = skill.Id,
            Skill = skill.Skill
        }).ToList();
        
        await _cacheService.SetCacheValueAsync(cacheKey, JsonSerializer.Serialize(skillDtos));

        return skillDtos;
    }
}
