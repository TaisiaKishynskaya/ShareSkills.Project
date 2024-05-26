using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    public Task<RoleEntity?> GetUserRoleAsync(string userRole) => context.Roles.FirstOrDefaultAsync(x => x.Name == userRole);
    public Task<RoleEntity?> GetRoleAsync(Guid id) => context.Roles.FirstOrDefaultAsync(x => x.Id == id);
}