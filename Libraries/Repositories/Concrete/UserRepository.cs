using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Users.ToListAsync(cancellationToken);

    public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public void Insert(UserEntity user)
        => context.Users.Add(user);

    public void Remove(UserEntity user)
        => context.Users.Remove(user);
}