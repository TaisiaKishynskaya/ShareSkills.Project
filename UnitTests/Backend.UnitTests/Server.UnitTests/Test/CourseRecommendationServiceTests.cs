using App.Services.RecommendationSystem.Test;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Server.UnitTests.Test;

public class CourseRecommendationServiceTests
{
    private readonly AppDbContext _ctx;
    private readonly Mock<ICosineSimilarityService> _cosineMock;
    private readonly CourseRecommendationService _service;
    private static readonly Guid _userId = Guid.Parse("0a06b59a-1339-4e76-8781-6a90be83ee52");

    public CourseRecommendationServiceTests()
    {
        _ctx = new AppDbContext(new DbContextOptions<AppDbContext>());
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
        var res = _service.GetRecommendedCourses(_userId, 0);
        Assert.Empty(res);
    }

    [Fact]
    public void GetRecommendedCourses_ReturnsAll_WhenTopNGreaterThanAvailable()
    {
        var result = _service.GetRecommendedCourses(_userId, 10);
        var seen = _ctx.Grades
            .Where(g => g.Students.Any(s => s.Id == _userId))
            .SelectMany(g => g.Courses.Select(c => c.Id))
            .ToHashSet();
        Assert.All(result, c => Assert.DoesNotContain(c.Id, seen));
    }

    [Fact]
    public void GetRecommendedCourses_NewStudent_ReturnsEmptyOrCandidates()
    {
        var newStudent = Guid.NewGuid();
        var result = _service.GetRecommendedCourses(newStudent, 5);
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
            StudentId = _userId,
            TeacherId = teacher.Id,
            Rating = 5,
            Teacher = teacher
        });
        var service2 = new CourseRecommendationService(_ctx, _cosineMock.Object);
        var results = service2.GetRecommendedCourses(_userId, 5);
        var coursesByFav = _ctx.Courses.Where(c => c.Teachers.Any(t => t.Id == teacher.Id)).Select(c => c.Id);
        Assert.True(results.Select(c => c.Id).Intersect(coursesByFav).Any());
    }

    [Fact]
    public void GetRecommendedCourses_CanBeCalledMultipleTimes_ConsistentOrder()
    {
        var first = _service.GetRecommendedCourses(_userId, 3);
        var second = _service.GetRecommendedCourses(_userId, 3);
        Assert.Equal(first.Select(c => c.Id), second.Select(c => c.Id));
    }
}