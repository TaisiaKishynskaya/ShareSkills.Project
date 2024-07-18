using Libraries.Entities.Concrete;

namespace App.Services.Abstract;

public interface IRoleService
{
    Task AssignUserToRoleAsync(UserEntity user, string role);
    
    Task<string> GetRoleNameAsync(Guid id);
}