using App.Services.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class LevelServiceTests
{
    private readonly Mock<ILevelRepository> _levelRepositoryMock;
    private readonly LevelService _service;

    public LevelServiceTests()
    {
        _levelRepositoryMock = new Mock<ILevelRepository>();
        _service = new LevelService(_levelRepositoryMock.Object);
    }
    
    [Fact]
    public async Task AssignTeacherToLevelAsync_ShouldAssignLevel_WhenLevelExists()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid() };
        var levelName = "Advanced";
        var level = new LevelEntity { Id = Guid.NewGuid(), Name = levelName };

        _levelRepositoryMock.Setup(repo => repo.GetTeacherLevelAsync(levelName))
            .ReturnsAsync(level);

        // Act
        await _service.AssignTeacherToLevelAsync(teacher, levelName);

        // Assert
        Assert.Equal(level, teacher.Level);
        _levelRepositoryMock.Verify(repo => repo.GetTeacherLevelAsync(levelName), Times.Once);
    }

    [Fact]
    public async Task AssignTeacherToLevelAsync_ShouldThrowException_WhenLevelDoesNotExist()
    {
        // Arrange
        var teacher = new TeacherEntity() { Id = default };
        var levelName = "NonExistingLevel";

        _levelRepositoryMock.Setup(repo => repo.GetTeacherLevelAsync(levelName))
            .ReturnsAsync((LevelEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AssignTeacherToLevelAsync(teacher, levelName));

        _levelRepositoryMock.Verify(repo => repo.GetTeacherLevelAsync(levelName), Times.Once);
    }

}