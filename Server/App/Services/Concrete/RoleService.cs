using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task AssignUserToRoleAsync(UserEntity user, string role)
    {
        user.Role = await roleRepository.GetUserRoleAsync(role) ?? throw new Exception("Role doesn't exists");
    }

    public async Task<string> GetRoleNameAsync(Guid id)
    {
        var role = await roleRepository.GetRoleAsync(id) ?? throw new Exception("Role doesn't exists");
        return role.Name;
    }
}