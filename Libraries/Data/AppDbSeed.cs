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
        
        if (!context.Teachers.Any())
        {
            var teacher = new TeacherEntity
            {
                Id = Guid.NewGuid(),
                Rating = 0, // Set the default rating
                SkillId = new Guid("4b7920e7-fc7f-49ca-b297-1e6699888c3e"),
                ClassTimeId = new Guid("5b4b8b96-7490-4b3e-9519-09411be4bd74"), // Specify the class time ID
                LevelId = new Guid("447dc41c-c38a-48be-820e-8c6ab124fe8d"), // Specify the level ID
                UserId = new Guid("461a393b-304a-435b-9c85-b16127d51c7a") // You need to replace this with an existing user ID
            };
        
            context.Teachers.Add(teacher);
        
            context.SaveChanges();
        }
    }
}