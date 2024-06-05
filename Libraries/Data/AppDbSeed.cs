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
        
        // if (!context.Teachers.Any())
        // {
        //     var teacher = new TeacherEntity
        //     {
        //         Id = Guid.NewGuid(),
        //         Rating = 0, // Set the default rating
        //         SkillId = new Guid("c36935a1-27fd-49fe-931c-bc291bfd3f3c"),
        //         ClassTimeId = new Guid("2028a98d-caea-4206-b2f9-fbf6df6f457b"), // Specify the class time ID
        //         LevelId = new Guid("3825028c-6985-4423-9d94-3e57985e1e1b"), // Specify the level ID
        //         UserId = new Guid("197a37d2-7d1c-447d-823a-b9a7f490b407") // You need to replace this with an existing user ID
        //     };
        //     
        //     var teacher1 = new TeacherEntity
        //     {
        //         Id = Guid.NewGuid(),
        //         Rating = 0, // Set the default rating
        //         SkillId = new Guid("67c68c91-6938-413e-8dc9-62dee48e301b"),
        //         ClassTimeId = new Guid("2028a98d-caea-4206-b2f9-fbf6df6f457b"), // Specify the class time ID
        //         LevelId = new Guid("3825028c-6985-4423-9d94-3e57985e1e1b"), // Specify the level ID
        //         UserId = new Guid("6626fa39-0623-4717-8cd1-4810576aa585") // You need to replace this with an existing user ID
        //     };
        //     
        //     
        //
        //     context.Teachers.Add(teacher);
        //     context.Teachers.Add(teacher1);
        //
        //     context.SaveChanges();
        // }
    }
}