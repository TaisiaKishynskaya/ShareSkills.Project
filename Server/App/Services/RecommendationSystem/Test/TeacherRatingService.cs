using Libraries.Entities.Concrete;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Test
{
    // DTO for ML.NET training (only numeric features + Label)
    public class TeacherTrainData
    {
        [LoadColumn(0)] public float SkillsMatchCount;
        [LoadColumn(1)] public float TimeDiff;
        [LoadColumn(2)] public float RelevantCoursesCount;
        [LoadColumn(3), ColumnName("Label")] public float Rating;
    }

    // DTO for prediction mapping (holds metadata + features)
    public class TeacherPredictData
    {
        public Guid TeacherId;
        public Guid StudentId;
        public float SkillsMatchCount;
        public float TimeDiff;
        public float RelevantCoursesCount;
    }

    // Prediction output
    public class TeacherScorePrediction
    {
        [ColumnName("Score")] public float Score;
    }

    public class TeacherRatingService
    {
        private readonly FakeAppDbContext2 _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private DataViewSchema _schema;

        public TeacherRatingService(FakeAppDbContext2 context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0);
            TrainModel();
        }

        // Переобучение модели на свежих данных из базы
        public void TrainModel()
        {
            var teacherRatings = _context.TeacherRatings;
            var classTimeMap = _context.ClassTimes
                .Select((ct, idx) => new { ct.Id, Index = idx })
                .ToDictionary(x => x.Id, x => x.Index);

            // Вычисляем статистику по студентам
            var studentStats = teacherRatings
                .GroupBy(tr => tr.StudentId)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        avgTime: g
                            .Select(tr => classTimeMap[_context.Teachers.First(t => t.Id == tr.TeacherId).ClassTimeId])
                            .DefaultIfEmpty(0)
                            .Average(),
                        skills: g
                            .Select(tr => _context.Teachers.First(t => t.Id == tr.TeacherId).SkillId)
                            .ToHashSet()
                    )
                );

            // Формируем обучающий набор
            var trainingData = teacherRatings.Select(tr =>
            {
                var stats = studentStats[tr.StudentId];
                var teacher = _context.Teachers.First(t => t.Id == tr.TeacherId);
                float skillsMatch = stats.skills.Contains(teacher.SkillId) ? 1f : 0f;
                float timeDiff = Math.Abs(classTimeMap[teacher.ClassTimeId] - (float)stats.avgTime);
                float relCourses = teacher.Courses.Count(c => c.Teachers.Any(tch => stats.skills.Contains(tch.SkillId)));
                return new TeacherTrainData
                {
                    SkillsMatchCount = skillsMatch,
                    TimeDiff = timeDiff,
                    RelevantCoursesCount = relCourses,
                    Rating = tr.Rating
                };
            });

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(TeacherTrainData.SkillsMatchCount),
                    nameof(TeacherTrainData.TimeDiff),
                    nameof(TeacherTrainData.RelevantCoursesCount))
                .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            _model = pipeline.Fit(dataView);
            
            _mlContext.Model.Save(_model, dataView.Schema, "teachers.zip");
            _schema = dataView.Schema;
        }

        // Метод получения рекомендованных учителей
        public IList<TeacherEntity> GetRecommendedTeachers(Guid studentId, int count)
        {
            if (_model == null)
                TrainModel();

            var classTimeMap = _context.ClassTimes
                .Select((ct, idx) => new { ct.Id, Index = idx })
                .ToDictionary(x => x.Id, x => x.Index);

            var pastRatings = _context.TeacherRatings.Where(tr => tr.StudentId == studentId).ToList();
            var ratedTeacherIds = pastRatings.Select(tr => tr.TeacherId).ToHashSet();

            float avgTime = (float)pastRatings
                .Select(tr => classTimeMap[_context.Teachers.First(t => t.Id == tr.TeacherId).ClassTimeId])
                .DefaultIfEmpty(0)
                .Average();
            var skillsRated = pastRatings
                .Select(tr => _context.Teachers.First(t => t.Id == tr.TeacherId).SkillId)
                .ToHashSet();

            // Формируем данные для предсказания
            var predictionData = _context.Teachers
                .Where(t => !ratedTeacherIds.Contains(t.Id))
                .Select(t => new TeacherPredictData
                {
                    TeacherId = t.Id,
                    StudentId = studentId,
                    SkillsMatchCount = skillsRated.Contains(t.SkillId) ? 1f : 0f,
                    TimeDiff = Math.Abs(classTimeMap[t.ClassTimeId] - avgTime),
                    RelevantCoursesCount = t.Courses.Count(c => c.Teachers.Any(tch => skillsRated.Contains(tch.SkillId)))
                })
                .ToList();

            // Загружаем только числовые фичи
            var featureData = predictionData.Select(pd => new TeacherTrainData
            {
                SkillsMatchCount = pd.SkillsMatchCount,
                TimeDiff = pd.TimeDiff,
                RelevantCoursesCount = pd.RelevantCoursesCount,
                Rating = 0 // Label не нужен для предсказания
            });
            var predDv = _mlContext.Data.LoadFromEnumerable(featureData);

            // Предсказание
            var preds = _model.Transform(predDv);
            var scores = _mlContext.Data.CreateEnumerable<TeacherScorePrediction>(preds, reuseRowObject: false)
                .Select((p, idx) => new { predictionData[idx].TeacherId, p.Score })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .ToList();

            return scores.Select(x => _context.Teachers.First(t => t.Id == x.TeacherId)).ToList();
        }
    }

    // Фейковый контекст с данными
    /*public class FakeAppDbContext
    {
        public List<UserEntity> Users { get; } = new List<UserEntity>();
        public List<StudentEntity> Students { get; } = new List<StudentEntity>();
        public List<TeacherEntity> Teachers { get; } = new List<TeacherEntity>();
        public List<CourseEntity> Courses { get; } = new List<CourseEntity>();
        public List<GradeEntity> Grades { get; } = new List<GradeEntity>();
        public List<TeacherRatingEntity> TeacherRatings { get; } = new List<TeacherRatingEntity>();
        public List<SkillEntity> Skills { get; } = new List<SkillEntity>();
        public List<ClassTimeEntity> ClassTimes { get; } = new List<ClassTimeEntity>();
        public List<LevelEntity> Levels { get; } = new List<LevelEntity>();

        public FakeAppDbContext()
        {
            var rand = new Random(0);

            // Создаём скиллы
            for (int i = 1; i <= 5; i++)
                Skills.Add(new SkillEntity { Id = Guid.NewGuid(), Skill = $"Skill{i}" });

            // Время занятий
            var times = new[] { "Morning", "Afternoon", "Evening" };
            foreach (var name in times)
                ClassTimes.Add(new ClassTimeEntity { Id = Guid.NewGuid(), Name = name, Teachers = new List<TeacherEntity>() });

            // Уровни
            var levels = new[] { "Beginner", "Intermediate", "Advanced" };
            foreach (var name in levels)
                Levels.Add(new LevelEntity { Id = Guid.NewGuid(), Name = name, Teachers = new List<TeacherEntity>() });

            // Преподаватели
            for (int i = 0; i < 10; i++)
            {
                var user = new UserEntity { Id = Guid.NewGuid(), Name = $"Teacher{i}", Surname = "T", Email = "t@example.com", Password = "pwd", RoleId = Guid.Empty };
                var teacher = new TeacherEntity
                {
                    Id = user.Id,
                    UserId = user.Id,
                    User = user,
                    Rating = rand.NextDouble() * 5,
                    ClassTimeId = ClassTimes[rand.Next(ClassTimes.Count)].Id,
                    LevelId = Levels[rand.Next(Levels.Count)].Id,
                    SkillId = Skills[rand.Next(Skills.Count)].Id
                };
                user.Teacher = teacher;
                Users.Add(user);
                Teachers.Add(teacher);

                // Связь ClassTime и Level
                ClassTimes.First(ct => ct.Id == teacher.ClassTimeId).Teachers.Add(teacher);
                Levels.First(lv => lv.Id == teacher.LevelId).Teachers.Add(teacher);
            }

            // Курсы
            for (int i = 0; i < 20; i++)
            {
                var course = new CourseEntity { Id = Guid.NewGuid(), Name = $"Course{i}", Teachers = new List<TeacherEntity>(), Users = new List<UserEntity>() };
                // назначим 1-3 случайных преподавателей
                var assigned = Teachers.OrderBy(x => rand.Next()).Take(rand.Next(1,4)).ToList();
                course.Teachers = assigned;
                foreach (var t in assigned)
                    t.Courses.Add(course);
                Courses.Add(course);
            }

            // Студенты и их рейтинги
            for (int i = 0; i < 30; i++)
            {
                var user = new UserEntity { Id = Guid.NewGuid(), Name = $"Student{i}", Surname = "S", Email = "s@example.com", Password = "pwd", RoleId = Guid.Empty };
                var student = new StudentEntity { Id = user.Id, UserId = user.Id, User = user, Purpose = "Learn" };
                user.Student = student;
                Users.Add(user);
                Students.Add(student);

                // Рейтинги преподавателей
                var ratedTeachers = Teachers.OrderBy(x => rand.Next()).Take(rand.Next(1,5));
                foreach (var t in ratedTeachers)
                {
                    var tr = new TeacherRatingEntity { Id = Guid.NewGuid(), StudentId = student.Id, TeacherId = t.Id, Rating = rand.Next(1,6), Student = student, Teacher = t };
                    TeacherRatings.Add(tr);
                }

                // Оценки курсов (GradeEntity)
                var ratedCourses = Courses.OrderBy(x => rand.Next()).Take(rand.Next(1,5));
                foreach (var c in ratedCourses)
                {
                    var gr = new GradeEntity { Id = Guid.NewGuid(), Grade = rand.Next(1,6) };
                    gr.Students.Add(student);
                    gr.Courses.Add(c);
                    gr.Teachers.Add(c.Teachers.First());
                    Grades.Add(gr);
                    student.Grades.Add(gr);
                    c.Grades.Add(gr);
                    foreach (var tch in c.Teachers)
                        tch.Grades.Add(gr);
                    student.Courses.Add(c);
                    c.Users.Add(user);
                }
            }
        }
    }*/
    
    public class FakeAppDbContext
    {
        public List<UserEntity> Users { get; } = new List<UserEntity>();
        public List<StudentEntity> Students { get; } = new List<StudentEntity>();
        public List<TeacherEntity> Teachers { get; } = new List<TeacherEntity>();
        public List<CourseEntity> Courses { get; } = new List<CourseEntity>();
        public List<TeacherRatingEntity> TeacherRatings { get; } = new List<TeacherRatingEntity>();
        public List<ClassTimeEntity> ClassTimes { get; } = new List<ClassTimeEntity>();
        public List<SkillEntity> Skills { get; } = new List<SkillEntity>();
        public List<LevelEntity> Levels { get; } = new List<LevelEntity>();

        // Константы для конкретного тестового студента
        public readonly Guid StudentXId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        public FakeAppDbContext()
        {
            // Навыки
            var skillA = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillA" };
            var skillB = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillB" };
            Skills.AddRange(new[] { skillA, skillB });

            // Время занятий
            var morning = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Morning", Teachers = new List<TeacherEntity>() };
            var evening = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Evening", Teachers = new List<TeacherEntity>() };
            ClassTimes.AddRange(new[] { morning, evening });

            // Уровни (один уровень, т.к. не используется в логике)
            var level = new LevelEntity { Id = Guid.NewGuid(), Name = "Beginner", Teachers = new List<TeacherEntity>() };
            Levels.Add(level);

            // Преподаватели
            var teacher1 = new TeacherEntity
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserId = Guid.NewGuid(),
                Rating = 4.5,
                ClassTimeId = morning.Id,
                ClassTime = morning,
                LevelId = level.Id,
                Level = level,
                SkillId = skillA.Id,
                Skill = skillA,
                Courses = new List<CourseEntity>(),
                Grades = new List<GradeEntity>()
            };
            var teacher2 = new TeacherEntity
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserId = Guid.NewGuid(),
                Rating = 2.0,
                ClassTimeId = evening.Id,
                ClassTime = evening,
                LevelId = level.Id,
                Level = level,
                SkillId = skillB.Id,
                Skill = skillB,
                Courses = new List<CourseEntity>(),
                Grades = new List<GradeEntity>()
            };
            var teacher3 = new TeacherEntity
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                UserId = Guid.NewGuid(),
                Rating = 3.5,
                ClassTimeId = morning.Id,
                ClassTime = morning,
                LevelId = level.Id,
                Level = level,
                SkillId = skillA.Id,
                Skill = skillA,
                Courses = new List<CourseEntity>(),
                Grades = new List<GradeEntity>()
            };
            var teacher4 = new TeacherEntity
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                UserId = Guid.NewGuid(),
                Rating = 3.0,
                ClassTimeId = evening.Id,
                ClassTime = evening,
                LevelId = level.Id,
                Level = level,
                SkillId = skillB.Id,
                Skill = skillB,
                Courses = new List<CourseEntity>(),
                Grades = new List<GradeEntity>()
            };
            Teachers.AddRange(new[] { teacher1, teacher2, teacher3, teacher4 });
            morning.Teachers.AddRange(new[] { teacher1, teacher3 });
            evening.Teachers.AddRange(new[] { teacher2, teacher4 });
            level.Teachers.AddRange(Teachers);

            // Курсы
            var course1 = new CourseEntity { Id = Guid.NewGuid(), Name = "Course1", Teachers = new List<TeacherEntity> { teacher1 }, Users = new List<UserEntity>()};
            var course2 = new CourseEntity { Id = Guid.NewGuid(), Name = "Course2", Teachers = new List<TeacherEntity> { teacher1, teacher2 }, Users = new List<UserEntity>()};
            var course3 = new CourseEntity { Id = Guid.NewGuid(), Name = "Course3", Teachers = new List<TeacherEntity> { teacher2 }, Users = new List<UserEntity>()};
            Courses.AddRange(new[] { course1, course2, course3 });
            teacher1.Courses.ToList().AddRange(new[] { course1, course2 });
            teacher2.Courses.ToList().AddRange(new[] { course2, course3 });
            teacher3.Courses = new List<CourseEntity>(); // нет курсов
            teacher4.Courses = new List<CourseEntity>();

            // Студент X
            var userX = new UserEntity { Id = StudentXId, Name = "StudentX", Surname = "Test", Email = "x@test.com", Password = "pwd", RoleId = Guid.Empty };
            var studentX = new StudentEntity { Id = StudentXId, UserId = StudentXId, User = userX, Purpose = "Test" };
            userX.Student = studentX;
            Users.Add(userX);
            Students.Add(studentX);

            // Оценки преподавателей StudentX: teacher1=5, teacher2=2
            TeacherRatings.Add(new TeacherRatingEntity { Id = Guid.NewGuid(), StudentId = StudentXId, TeacherId = teacher1.Id, Rating = 5, Student = studentX, Teacher = teacher1 });
            TeacherRatings.Add(new TeacherRatingEntity { Id = Guid.NewGuid(), StudentId = StudentXId, TeacherId = teacher2.Id, Rating = 2, Student = studentX, Teacher = teacher2 });

            // Оценки курсов StudentX: course1=4, course3=3
            var grade1 = new GradeEntity
            {
                Id = Guid.NewGuid(),
                Grade = 4
            };
            grade1.Students.Add(studentX); grade1.Courses.Add(course1); grade1.Teachers.Add(teacher1);
            studentX.Grades.Add(grade1); course1.Grades.Add(grade1); teacher1.Grades.Add(grade1);
            course1.Users.Add(userX);

            var grade3 = new GradeEntity
            {
                Id = Guid.NewGuid(),
                Grade = 3
            };
            grade3.Students.Add(studentX); grade3.Courses.Add(course3); grade3.Teachers.Add(teacher2);
            studentX.Grades.Add(grade3); course3.Grades.Add(grade3); teacher2.Grades.Add(grade3);
            course3.Users.Add(userX);
        }
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
                SkillId = sk.Id, Skill = sk,
                //Courses = new(), Grades = new()
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

}
