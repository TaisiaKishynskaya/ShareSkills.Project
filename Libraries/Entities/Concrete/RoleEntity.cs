namespace Libraries.Entities.Concrete;

public class RoleEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<UserEntity> Users { get; set; }
}