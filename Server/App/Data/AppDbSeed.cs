using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

// Тут наши начальные данные.
// ЭТО ПРИМЕР работы этого класса, вы эти данные удалите и начнете писать свой гибкий код
public class AppDbSeed
{
    public static void Seed(ModelBuilder builder)
    {
        var users = new List<UserEntity>
        {
            new()
            {
                Id = 1, 
                Name = "Bob", 
                Surname = "Smith",
                Email = "bobsmith@gmail.com",
                Role = "Student",
                Skills = new List<SkillEntity>(), //here should be list/etc of skills, that user choose, fix it
                Objective = string.Empty,
                Password = "HashCode,Boys!"
            },
            
            new()
            {
                Id = 2, 
                Name = "Alice", 
                Surname = "Smith", 
                Email = "alicesmith@gmail.com",
                Skills = new List<SkillEntity>(), // here should be list/etc of skills, that user choose, fix it
                Role = "Teacher",
                Objective = "I like math and",
                Password = "HashCode,Boys!",
            },
            
            // new() { ... }
        };


        var students = new List<StudentEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                User = users[1], // because we have "required" UserEntity User in StudentEntity. If u don`t want required - don`t write this
                Meetings = new List<MeetingEntity>(),
                Grades = new List<GradeEntity>(),
                Purpose = "Here purposes" 
            }
            
            // new() { ... }
        };
        
        
        var teachers = new List<TeacherEntity>
        {
            new()
            {
                Id = 1,
                UserId = 2,
                Meetings = new List<MeetingEntity>(),
                Grades = new List<GradeEntity>(),
                Rating = 1.9,
            }
            
            // new() { ... }
        };
        
        
        var skills = new List<SkillEntity>
        {
            new()
            {
                Id = 1,
                Skill = "Math",
                Users = users, // here should be users (or users' id) who have "Math" in skills, fix it
            }
            
            // new() { ... }
        };
        
        
        var meetings = new List<MeetingEntity>
        {
            new()
            {
                Id = 1,
                DateAndTime = DateTime.UtcNow,
                TeacherId = 1,
                StudentId = 1,
            }
            
            // new() { ... }
        };
        
        
        var grades = new List<GradeEntity>
        {
            new()
            {
                Id = 1,
                Grade = 1,
                Teachers = teachers,  // here should be teachers (or teachers' id) who have this grade, fix it
                Students = students,  // here should be students (or students' id) who have this grade, fix it
            },
            
            // new() { ... }
            
            new()
            {
                Id = 5,
                Grade = 5,
                Teachers = teachers,  // here should be teachers (or teachers' id) who have this grade, fix it
                Students = students,  // here should be students (or students' id) who have this grade, fix it
            }
        };
        
        
        builder.Entity<UserEntity>().HasData(users); // указывает EF Core добавить начальные данные (seed data) для сущности CatalogTypeEntity. 
        builder.Entity<StudentEntity>().HasData(students);
        builder.Entity<StudentEntity>().HasData(teachers);
        builder.Entity<TeacherEntity>().HasData(skills);
        builder.Entity<TeacherEntity>().HasData(meetings);
        builder.Entity<TeacherEntity>().HasData(grades);
    }
}