using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IRoleRepository
{
    Task<RoleEntity?> GetUserRoleAsync(string userRole);

    Task<RoleEntity?> GetRoleAsync(Guid id);
}