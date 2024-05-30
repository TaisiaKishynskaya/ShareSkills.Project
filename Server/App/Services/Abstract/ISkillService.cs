using Libraries.Contracts.Skill;

namespace App.Services.Abstract;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SkillDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SkillDto> CreateAsync(SkillForCreatingDto studentForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, SkillForUpdateDto studentForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}