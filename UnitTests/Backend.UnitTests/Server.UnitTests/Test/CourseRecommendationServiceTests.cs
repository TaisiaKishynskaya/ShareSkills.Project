using App.Services.RecommendationSystem.Test;
using Libraries.Entities.Concrete;
using Moq;

namespace Server.UnitTests.Test;

public class CourseRecommendationServiceTests
{
    private readonly FakeAppDbContext _ctx;
    private readonly Mock<ICosineSimilarityService> _cosineMock;
    private readonly CourseRecommendationService _service;

    public CourseRecommendationServiceTests()
    {
        _ctx = new FakeAppDbContext();
        // Настроим несколько курсов и преподавательских рейтингов вручную если нужно

        // Мокаем косинус-сходство: просто возвращаем совпадение признаков
        _cosineMock = new Mock<ICosineSimilarityService>();
        _cosineMock.Setup(c => c.Compute(It.IsAny<float[]>(), It.IsAny<float[]>()))
                   .Returns<float[], float[]>((a, b) =>
                       a.Zip(b, (x, y) => x * y).Sum());

        _service = new CourseRecommendationService(_ctx, _cosineMock.Object);
    }

    [Fact]
    public void Constructor_TrainModel_CreatesModelFile()
    {
        Assert.True(System.IO.File.Exists("courses.zip"));
    }

    [Fact]
    public void GetRecommendedCourses_ReturnsEmpty_WhenTopNZero()
    {
        var res = _service.GetRecommendedCourses(_ctx.StudentXId, 0);
        Assert.Empty(res);
    }

    [Fact]
    public void GetRecommendedCourses_ReturnsAll_WhenTopNGreaterThanAvailable()
    {
        // У студента X в FakeAppDbContext есть оценки по двум курсам
        // Остальные подходят по навыкам SkillA (курсы 2,4,5) и SkillB (курс6)
        var result = _service.GetRecommendedCourses(_ctx.StudentXId, 10);
        // Ожидаем, что метод выдаст не более всех кандидатов
        Assert.True(result.Count <= _ctx.Courses.Count);
        // Убедимся, что не возвращаются уже пройденные курсы
        var seen = _ctx.Grades
            .Where(g => g.Students.Any(s => s.Id == _ctx.StudentXId))
            .SelectMany(g => g.Courses.Select(c => c.Id))
            .ToHashSet();
        Assert.All(result, c => Assert.DoesNotContain(c.Id, seen));
    }

    [Fact]
    public void GetRecommendedCourses_NewStudent_ReturnsEmptyOrCandidates()
    {
        var newStudent = Guid.NewGuid();
        var result = _service.GetRecommendedCourses(newStudent, 5);
        // У нового студента нет оценок -> нет prefSkills -> нет кандидатов
        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommendedCourses_ConsidersFavoriteTeachers()
    {
        // Добавим высокие рейтинги преподавателей X
        var teacher = _ctx.Teachers.First();
        _ctx.TeacherRatings.Add(new TeacherRatingEntity
        {
            Id = Guid.NewGuid(),
            StudentId = _ctx.StudentXId,
            TeacherId = teacher.Id,
            Rating = 5,
            Teacher = teacher
        });
        // Теперь пересоздадим сервис чтобы учесть новые рейтинги
        var service2 = new CourseRecommendationService(_ctx, _cosineMock.Object);
        var results = service2.GetRecommendedCourses(_ctx.StudentXId, 5);
        // Курсы, где этот учитель преподает, должны подняться выше в списке
        var coursesByFav = _ctx.Courses.Where(c => c.Teachers.Any(t => t.Id == teacher.Id)).Select(c => c.Id);
        Assert.True(results.Select(c => c.Id).Intersect(coursesByFav).Any());
    }

    [Fact]
    public void GetRecommendedCourses_CanBeCalledMultipleTimes_ConsistentOrder()
    {
        var first = _service.GetRecommendedCourses(_ctx.StudentXId, 3);
        var second = _service.GetRecommendedCourses(_ctx.StudentXId, 3);
        Assert.Equal(first.Select(c => c.Id), second.Select(c => c.Id));
    }
}