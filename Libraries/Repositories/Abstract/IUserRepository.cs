using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Insert(UserEntity user);
    void Remove(UserEntity user);
}