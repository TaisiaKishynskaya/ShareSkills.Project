using App.Services.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class ClassTimeServiceTests
{
    private readonly Mock<IClassTimeRepository> _classTimeRepositoryMock;
    private readonly ClassTimeService _service;

    public ClassTimeServiceTests()
    {
        _classTimeRepositoryMock = new Mock<IClassTimeRepository>();
        _service = new ClassTimeService(_classTimeRepositoryMock.Object);
    }

    [Fact]
    public async Task AssignTeacherToClassTimeAsync_ShouldAssignClassTime_WhenClassTimeExists()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid() };
        var classTime = "Morning";
        var classTimeEntity = new ClassTimeEntity { Id = Guid.NewGuid(), Name = classTime };

        _classTimeRepositoryMock.Setup(repo => repo.GetTeacherClassTimeAsync(classTime))
                                .ReturnsAsync(classTimeEntity);

        // Act
        await _service.AssignTeacherToClassTimeAsync(teacher, classTime);

        // Assert
        Assert.Equal(classTimeEntity, teacher.ClassTime); // Ensure the class time was assigned correctly

        _classTimeRepositoryMock.Verify(repo => repo.GetTeacherClassTimeAsync(classTime), Times.Once);
    }

    [Fact]
    public async Task AssignTeacherToClassTimeAsync_ShouldThrowException_WhenClassTimeDoesNotExist()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid() };
        var classTime = "NonExistentClassTime";

        _classTimeRepositoryMock.Setup(repo => repo.GetTeacherClassTimeAsync(classTime))
                                .ReturnsAsync((ClassTimeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AssignTeacherToClassTimeAsync(teacher, classTime));

        _classTimeRepositoryMock.Verify(repo => repo.GetTeacherClassTimeAsync(classTime), Times.Once);
    }

    [Fact]
    public async Task GetClassTimeNameAsync_ShouldReturnClassTimeName_WhenClassTimeExists()
    {
        // Arrange
        var classTimeId = Guid.NewGuid();
        var classTimeName = "Morning";
        var classTimeEntity = new ClassTimeEntity { Id = classTimeId, Name = classTimeName };

        _classTimeRepositoryMock.Setup(repo => repo.GetClassTimeAsync(classTimeId))
                                .ReturnsAsync(classTimeEntity);

        // Act
        var result = await _service.GetClassTimeNameAsync(classTimeId);

        // Assert
        Assert.Equal(classTimeName, result); // Ensure the class time name was returned correctly

        _classTimeRepositoryMock.Verify(repo => repo.GetClassTimeAsync(classTimeId), Times.Once);
    }

    [Fact]
    public async Task GetClassTimeNameAsync_ShouldThrowException_WhenClassTimeDoesNotExist()
    {
        // Arrange
        var classTimeId = Guid.NewGuid();

        _classTimeRepositoryMock.Setup(repo => repo.GetClassTimeAsync(classTimeId))
                                .ReturnsAsync((ClassTimeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetClassTimeNameAsync(classTimeId));

        _classTimeRepositoryMock.Verify(repo => repo.GetClassTimeAsync(classTimeId), Times.Once);
    }
}