namespace Libraries.Entities.Concrete;

public class RoleEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    //1-*
    public List<UserEntity> Users { get; set; }
}