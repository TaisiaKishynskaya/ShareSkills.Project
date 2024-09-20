using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Contracts.Grade;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class GradeServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITeacherService> _teacherServiceMock;
    private readonly GradeService _service;

    public GradeServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _teacherServiceMock = new Mock<ITeacherService>();
        _service = new GradeService(_unitOfWorkMock.Object, _teacherServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateGradeAndRecountTotalGrade_WhenCalled()
    {
        // Arrange
        var gradeForCreatingDto = new GradeForCreatingDto
        {
            TeacherId = Guid.NewGuid(),
            Grade = 85
        };
        var existingScores = new List<int> { 80, 90 };

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetScoresByTeacherIdAsync(gradeForCreatingDto.TeacherId))
            .ReturnsAsync(existingScores);

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.Insert(It.IsAny<GradeEntity>()));

        // Act
        var result = await _service.CreateAsync(gradeForCreatingDto);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.GradeRepository.Insert(It.IsAny<GradeEntity>()), Times.Once);
        _teacherServiceMock.Verify(ts => ts.RecountTotalGradeAsync(gradeForCreatingDto.TeacherId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(85, result.Grade);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldCalculateCorrectAverageGrade_WhenNewGradeIsAdded()
    {
        // Arrange
        var gradeForCreatingDto = new GradeForCreatingDto
        {
            TeacherId = Guid.NewGuid(),
            Grade = 70
        };
        var existingScores = new List<int> { 60, 80 };

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetScoresByTeacherIdAsync(gradeForCreatingDto.TeacherId))
            .ReturnsAsync(existingScores);

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.Insert(It.IsAny<GradeEntity>()));

        // Act
        var result = await _service.CreateAsync(gradeForCreatingDto);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.GradeRepository.Insert(It.IsAny<GradeEntity>()), Times.Once);
        _teacherServiceMock.Verify(ts => ts.RecountTotalGradeAsync(gradeForCreatingDto.TeacherId, 70, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(70, result.Grade);
    }
    
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveGrade_WhenGradeExists()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var grade = new GradeEntity { Id = gradeId, Grade = 90 };

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(grade);

        // Act
        await _service.DeleteAsync(gradeId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.GradeRepository.Remove(grade), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowGradeNotFoundException_WhenGradeDoesNotExist()
    {
        // Arrange
        var gradeId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GradeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<GradeNotFoundException>(() => _service.DeleteAsync(gradeId));

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.Remove(It.IsAny<GradeEntity>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGrades()
    {
        // Arrange
        var grades = new List<GradeEntity>
        {
            new GradeEntity { Id = Guid.NewGuid(), Grade = 85 },
            new GradeEntity { Id = Guid.NewGuid(), Grade = 90 }
        };

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(grades);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, dto => dto.Grade == 85);
        Assert.Contains(result, dto => dto.Grade == 90);

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoGradesExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GradeEntity>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnGradeDto_WhenGradeExists()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var gradeEntity = new GradeEntity { Id = gradeId, Grade = 85 };

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gradeEntity);

        // Act
        var result = await _service.GetByIdAsync(gradeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gradeId, result.Id);
        Assert.Equal(85, result.Grade);

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowGradeNotFoundException_WhenGradeDoesNotExist()
    {
        // Arrange
        var gradeId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GradeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<GradeNotFoundException>(() => _service.GetByIdAsync(gradeId));

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateGrade_WhenGradeExists()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var newGrade = 90;
        var gradeEntity = new GradeEntity { Id = gradeId, Grade = 85 };

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(gradeEntity);

        // Act
        await _service.UpdateAsync(gradeId, new GradeForUpdateDto { Grade = newGrade });

        // Assert
        Assert.Equal(newGrade, gradeEntity.Grade); // Ensure the grade was updated

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowGradeNotFoundException_WhenGradeDoesNotExist()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var newGrade = 90;

        _unitOfWorkMock.Setup(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GradeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<GradeNotFoundException>(() => _service.UpdateAsync(gradeId, new GradeForUpdateDto { Grade = newGrade }));

        _unitOfWorkMock.Verify(uow => uow.GradeRepository.GetByIdAsync(gradeId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); // SaveChangesAsync should not be called
    }
}