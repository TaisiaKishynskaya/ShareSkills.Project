using Libraries.Entities.Concrete;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Test;
    // Для обучения: только числовые признаки + Label
public class CourseTrainData
{
    [LoadColumn(0)] public float SkillsMatchCount;
    [LoadColumn(1)] public float FavTeacherCount;
    [LoadColumn(2), ColumnName("Label")] public float Rating;
}

// Для предсказания: мета + признаки
public class CoursePredictData
{
    public Guid CourseId;
    public Guid StudentId;
    public float SkillsMatchCount;
    public float FavTeacherCount;
}

// Выход модели
public class CourseScorePrediction
{
    [ColumnName("Score")] public float Score;
}

public class CourseRecommendationService
{
    private readonly FakeAppDbContext2 _ctx;
    private readonly MLContext _ml;
    private ITransformer _model;
    private DataViewSchema _schema;
    private const string _path = "courses.zip";

    public CourseRecommendationService(FakeAppDbContext2 ctx)
    {
        _ctx = ctx;
        _ml = new MLContext(seed:0);
        TrainModel();
    }

    public void TrainModel()
    {
        // Собираем все рейтинги курсов (GradeEntity) в CourseTrainData
        var data = new List<CourseTrainData>();
        foreach (var gr in _ctx.Grades)
        {
            // берем первого преподавателя курса для простоты
            var course = gr.Courses.First();
            var student = gr.Students.First();
            // SkillsMatchCount и FavTeacherCount считаем произвольно 0, т.к. модель учится на чистых рейтингах
            data.Add(new CourseTrainData {
                SkillsMatchCount = 0,
                FavTeacherCount  = 0,
                Rating           = gr.Grade
            });
        }

        var dv = _ml.Data.LoadFromEnumerable(data);
        var pipe = _ml.Transforms.Concatenate("Features",
                        nameof(CourseTrainData.SkillsMatchCount),
                        nameof(CourseTrainData.FavTeacherCount))
                   .Append(_ml.Regression.Trainers.FastTree(labelColumnName:"Label", featureColumnName:"Features"));
        _model = pipe.Fit(dv);
        _ml.Model.Save(_model, dv.Schema, _path);
        _schema = dv.Schema;
    }

    public IList<CourseEntity> GetRecommendedCourses(Guid studentId, int topN = 5)
    {
        if (_model == null) TrainModel();

        // 1) Курсы, которые студент уже оценил
        var seen = _ctx.Grades
            .Where(g => g.Students.Any(s => s.Id == studentId))
            .SelectMany(g => g.Courses.Select(c=>c.Id))
            .ToHashSet();

        // 2) Навыки из курсов, оцененных >=3
        var prefSkills = _ctx.Grades
            .Where(g => g.Grade >= 3 && g.Students.Any(s => s.Id==studentId))
            .SelectMany(g => g.Courses)
            .SelectMany(c => c.Teachers.Select(t=>t.SkillId))
            .ToHashSet();

        // 3) Любимые преподаватели (rating>=4)
        var favT = _ctx.TeacherRatings
            .Where(r=>r.StudentId==studentId && r.Rating>=4)
            .Select(r=>r.TeacherId).ToHashSet();

        // 4) Кандидатные курсы
        var cands = _ctx.Courses
            .Where(c=>!seen.Contains(c.Id) && c.Teachers.Any(t=>prefSkills.Contains(t.SkillId)))
            .ToList();

        // Формируем предсказания
        var pd = new List<CoursePredictData>();
        foreach(var c in cands)
        {
            pd.Add(new CoursePredictData {
                CourseId = c.Id,
                StudentId = studentId,
                SkillsMatchCount = c.Teachers.Count(t=>prefSkills.Contains(t.SkillId)),
                FavTeacherCount  = c.Teachers.Count(t=>favT.Contains(t.Id))
            });
        }

        // Преобразуем в CourseTrainData для ML
        var fd = pd.Select(x=> new CourseTrainData {
            SkillsMatchCount = x.SkillsMatchCount,
            FavTeacherCount  = x.FavTeacherCount,
            Rating = 0
        });
        var dv2 = _ml.Data.LoadFromEnumerable(fd);
        var preds = _model.Transform(dv2);
        var scores = _ml.Data.CreateEnumerable<CourseScorePrediction>(preds, reuseRowObject:false)
            .Select((p,i)=> new { pd[i].CourseId, p.Score })
            .OrderByDescending(x=>x.Score)
            .Take(topN)
            .Select(x=> _ctx.Courses.First(c=>c.Id==x.CourseId))
            .ToList();

        return scores;
    }
    
    public class FakeAppDbContext2
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

    public FakeAppDbContext2()
    {
        // 1) Skills
        var skillA = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillA" };
        var skillB = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillB" };
        Skills.AddRange(new[]{ skillA, skillB });

        // 2) ClassTimes
        var morning = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Morning", Teachers = new List<TeacherEntity>() };
        var evening = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Evening", Teachers = new List<TeacherEntity>() };
        ClassTimes.AddRange(new[]{ morning, evening });

        // 3) Level
        var lvl = new LevelEntity { Id = Guid.NewGuid(), Name = "Beginner", Teachers = new List<TeacherEntity>() };
        Levels.Add(lvl);

        // 4) Teachers
        TeacherEntity makeT(string id, SkillEntity sk, ClassTimeEntity ct) {
            var t = new TeacherEntity {
                Id = Guid.Parse(id),
                UserId = Guid.NewGuid(),
                Rating = 1.0,
                ClassTimeId = ct.Id,
                ClassTime = ct,
                LevelId = lvl.Id,
                Level = lvl,
                SkillId = sk.Id,
                Skill = sk,
                Courses = new List<CourseEntity>(),
                Grades = new List<GradeEntity>()
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
                Id = Guid.NewGuid(),
                Name = name,
                Teachers = tch.ToList(),
                Users = new List<UserEntity>(),
                Grades = new List<GradeEntity>()
            };
            Courses.Add(c);
            foreach(var t in tch) t.Courses.Add(c);
            return c;
        }
        var course1 = makeC("Course1", teacher1);
        var course2 = makeC("Course2", teacher1, teacher2);
        var course3 = makeC("Course3", teacher2);
        var course4 = makeC("Course4", teacher3);
        var course5 = makeC("Course5", teacher3);
        var course6 = makeC("Course6", teacher4);

        // 6) Student X
        var ux = new UserEntity { Id = StudentXId, Name="X",Surname="X",Email="x@x",Password="p",RoleId=Guid.Empty,
            Courses = new List<CourseEntity>(),
            Skills = new List<SkillEntity>(),
            Meetings = new List<MeetingEntity>() };
        var sx = new StudentEntity { Id = StudentXId, UserId=StudentXId, User=ux, Purpose="test", Grades = new List<GradeEntity>() };
        ux.Student = sx;
        Users.Add(ux); Students.Add(sx);

        // 7) TeacherRatings by X
        TeacherRatings.Add(new TeacherRatingEntity {
            Id=Guid.NewGuid(), StudentId=StudentXId, TeacherId=teacher1.Id, Rating=5, Student=sx, Teacher=teacher1 });
        TeacherRatings.Add(new TeacherRatingEntity {
            Id=Guid.NewGuid(), StudentId=StudentXId, TeacherId=teacher2.Id, Rating=2, Student=sx, Teacher=teacher2 });

        // 8) Course grades by X
        GradeEntity mkG(int gr, CourseEntity c, TeacherEntity t) {
            var g = new GradeEntity { Id=Guid.NewGuid(), Grade=gr, Students = new List<StudentEntity>(), Courses = new List<CourseEntity>(), Teachers = new List<TeacherEntity>() };
            g.Students.Add(sx);
            g.Courses.Add(c);
            g.Teachers.Add(t);
            sx.Grades.Add(g);
            c.Grades.Add(g);
            t.Grades.Add(g);
            c.Users.Add(ux);
            Grades.Add(g);
            return g;
        }
        mkG(4, course1, teacher1);
        mkG(3, course3, teacher2);
    }
}

}