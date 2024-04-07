using Libraries.Contracts.User;

namespace App.Services.Abstract;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserForCreationDto teacherForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UserForUpdateDto teacherForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}