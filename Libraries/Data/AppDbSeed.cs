using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Data;

// Тут наши начальные данные.
// ЭТО ПРИМЕР работы этого класса, вы эти данные удалите и начнете писать свой гибкий код
public class AppDbSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Skills.Any())
        {
            context.Skills.AddRange(
                new SkillEntity { Id = Guid.NewGuid(), Skill = "C#" },
                new SkillEntity { Id = Guid.NewGuid(), Skill = "ASP.NET" },
                new SkillEntity { Id = Guid.NewGuid(), Skill = "Entity Framework" },
                new SkillEntity { Id = Guid.NewGuid(), Skill = "SQL" },
                new SkillEntity { Id = Guid.NewGuid(), Skill = "JavaScript" }
            );

            context.SaveChanges();
        }
        
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new RoleEntity { Id = Guid.NewGuid(), Name = "Student" },
                new RoleEntity { Id = Guid.NewGuid(), Name = "Teacher" }
            );

            context.SaveChanges();
        }
        
        if (!context.ClassTimes.Any())
        {
            context.ClassTimes.AddRange(
                new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Morning" },
                new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Day" }, 
                new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Evening" }
            );

            context.SaveChanges();
        }
        
        if (!context.Levels.Any())
        {
            context.Levels.AddRange(
                new LevelEntity { Id = Guid.NewGuid(), Name = "Introductory" },
                new LevelEntity { Id = Guid.NewGuid(), Name = "Intermediate" }, 
                new LevelEntity { Id = Guid.NewGuid(), Name = "Advanced" }
            );

            context.SaveChanges();
        }
    }
}