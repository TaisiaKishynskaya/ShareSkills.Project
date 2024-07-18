using Libraries.Contracts.Grade;

namespace App.Services.Abstract;

public interface IGradeService
{
    Task<IEnumerable<GradeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GradeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GradeDto> CreateAsync(GradeForCreatingDto studentForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, GradeForUpdateDto studentForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}