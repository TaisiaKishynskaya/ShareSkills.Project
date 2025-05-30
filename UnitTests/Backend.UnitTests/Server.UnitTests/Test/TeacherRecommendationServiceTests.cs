using App.Services.RecommendationSystem.Test;
using Moq;

namespace Server.UnitTests.Test;

public class TeacherRecommendationServiceTests
{
    private readonly FakeAppDbContext _context;
    private readonly Mock<ICosineSimilarityService> _cosineMock;
    private readonly TeacherRatingService _service;

    public TeacherRecommendationServiceTests()
    {
        _context = new FakeAppDbContext();
        // Заглушка косинус-сходства всегда возвращает 0
        _cosineMock = new Mock<ICosineSimilarityService>();
        _cosineMock.Setup(c => c.Compute(It.IsAny<float[]>(), It.IsAny<float[]>()))
                    .Returns(0f);
        _service = new TeacherRatingService(_context, _cosineMock.Object);
    }

    [Fact]
    public void Constructor_TrainModel_DoesNotThrow()
    {
        // Конструктор уже вызывается в фикстуре
        // Просто убеждаемся, что модель сохранена на диск
        Assert.True(File.Exists("teachers.zip"));
    }

    [Fact]
    public void GetRecommendedTeachers_ReturnsEmpty_WhenCountZero()
    {
        var result = _service.GetRecommendedTeachers(_context.StudentXId, 0);
        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommendedTeachers_ReturnsAll_WhenCountGreaterThanAvailable()
    {
        // В контексте два оцененных (teacher1, teacher2) → остаётся 2 свободных
        var result = _service.GetRecommendedTeachers(_context.StudentXId, 10);
        var expected = new[]
        {
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Contains(t.Id, expected));
    }

    [Fact]
    public void GetRecommendedTeachers_NewStudent_ReturnsCorrectCount()
    {
        var newStudent = Guid.NewGuid();
        var result = _service.GetRecommendedTeachers(newStudent, 3);
        // Никаких прошлых рейтингов → доступны все 4 преподавателя
        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Select(t => t.Id).Distinct().Count());
    }

    [Fact]
    public void GetRecommendedTeachers_NegativeCount_ReturnsEmpty()
    {
        var result = _service.GetRecommendedTeachers(_context.StudentXId, -5);
        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommendedTeachers_CanBeCalledMultipleTimes()
    {
        var first = _service.GetRecommendedTeachers(_context.StudentXId, 2);
        var second = _service.GetRecommendedTeachers(_context.StudentXId, 2);
        // Два последовательных вызова не должны ломаться
        Assert.Equal(first.Select(t => t.Id), second.Select(t => t.Id));
    }
}