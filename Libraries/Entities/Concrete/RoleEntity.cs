using Microsoft.AspNetCore.Identity;

namespace Libraries.Entities.Concrete;

public class RoleEntity : IdentityRole<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    //1-*
    public List<UserEntity> Users { get; set; }
}