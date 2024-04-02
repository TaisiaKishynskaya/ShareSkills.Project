namespace Libraries.Entities.Concrete;

public class SkillEntity
{
    public int Id { get; set; }
    public string Skill { get; set; }
    
    // *-*
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}