using Libraries.Entities.Concrete;

namespace App.Services.RecommendationSystem.Test;

public class FakeAppDbContext
{
    public List<UserEntity> Users { get; } = new List<UserEntity>();
    public List<StudentEntity> Students { get; } = new List<StudentEntity>();
    public List<TeacherEntity> Teachers { get; } = new List<TeacherEntity>();
    public List<CourseEntity> Courses { get; } = new List<CourseEntity>();
    public List<TeacherRatingEntity> TeacherRatings { get; } = new List<TeacherRatingEntity>();
    public List<GradeEntity> Grades { get; } = new List<GradeEntity>();
    public List<ClassTimeEntity> ClassTimes { get; } = new List<ClassTimeEntity>();
    public List<SkillEntity> Skills { get; } = new List<SkillEntity>();
    public List<LevelEntity> Levels { get; } = new List<LevelEntity>();

    public readonly Guid StudentXId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    public FakeAppDbContext()
    {
        // 1) Skills
        var skillA = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillA" };
        var skillB = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillB" };
        Skills.AddRange(new[]{ skillA, skillB });

        // 2) ClassTimes
        var morning = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Morning", Teachers = new() };
        var evening = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Evening", Teachers = new() };
        ClassTimes.AddRange(new[]{ morning, evening });

        // 3) Level
        var lvl = new LevelEntity { Id = Guid.NewGuid(), Name = "Beginner", Teachers = new() };
        Levels.Add(lvl);

        // 4) Teachers
        TeacherEntity makeT(string id, SkillEntity sk, ClassTimeEntity ct) {
            var t = new TeacherEntity {
                Id = Guid.Parse(id), UserId = Guid.NewGuid(),
                Rating = 1.0, ClassTimeId = ct.Id, ClassTime = ct,
                LevelId = lvl.Id, Level = lvl,
                SkillId = sk.Id, Skill = sk
            };
            Teachers.Add(t);
            ct.Teachers.Add(t);
            lvl.Teachers.Add(t);
            return t;
        }
        var teacher1 = makeT("11111111-1111-1111-1111-111111111111", skillA, morning);
        var teacher2 = makeT("22222222-2222-2222-2222-222222222222", skillB, evening);
        var teacher3 = makeT("33333333-3333-3333-3333-333333333333", skillA, morning);
        var teacher4 = makeT("44444444-4444-4444-4444-444444444444", skillB, evening);

        // 5) Courses (добавлены course4–course6)
        CourseEntity makeC(string name, params TeacherEntity[] tch) {
            var c = new CourseEntity {
                Id      = Guid.NewGuid(),
                Name    = name,
                Teachers       = new List<TeacherEntity>(tch),   // инициализируем Teachers
                Users          = new List<UserEntity>(),         // инициализируем Users!
                Grades         = new List<GradeEntity>()         // инициализируем Grades
            };
            Courses.Add(c);
            foreach(var t in tch)
                t.Courses.Add(c);
            return c;
        }
        
        var course1 = makeC("Course1", teacher1);
        var course2 = makeC("Course2", teacher1, teacher2);
        var course3 = makeC("Course3", teacher2);
        var course4 = makeC("Course4", teacher3);
        var course5 = makeC("Course5", teacher3);
        var course6 = makeC("Course6", teacher4);

        // 6) Student X
        var ux = new UserEntity { Id = StudentXId, Name="X",Surname="X",Email="x@x",Password="p",RoleId=Guid.Empty };
        var sx = new StudentEntity { Id = StudentXId, UserId=StudentXId, User=ux, Purpose="test" };
        course1.Users.Add(ux);
        course3.Users.Add(ux);
        ux.Student = sx;
        Users.Add(ux); Students.Add(sx);

        // 7) TeacherRatings by X
        TeacherRatings.Add(new TeacherRatingEntity {
            Id=Guid.NewGuid(), StudentId=StudentXId, TeacherId=teacher1.Id, Rating=5, Student=sx, Teacher=teacher1 });
        TeacherRatings.Add(new TeacherRatingEntity {
            Id=Guid.NewGuid(), StudentId=StudentXId, TeacherId=teacher2.Id, Rating=2, Student=sx, Teacher=teacher2 });

        // 8) Course grades by X
        GradeEntity mkG(int gr, CourseEntity c, TeacherEntity t) {
            var g = new GradeEntity { Id=Guid.NewGuid(), Grade=gr };
            g.Students.Add(sx); g.Courses.Add(c); g.Teachers.Add(t);
            sx.Grades.Add(g); c.Grades.Add(g); t.Grades.Add(g); c.Users.Add(ux); Grades.Add(g);
            return g;
        }
        mkG(4, course1, teacher1);
        mkG(3, course3, teacher2);
    }
}