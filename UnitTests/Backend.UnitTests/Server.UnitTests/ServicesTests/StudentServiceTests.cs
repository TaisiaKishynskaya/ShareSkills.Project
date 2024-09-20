using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Concrete;
using Libraries.Contracts.Student;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private StudentService _service;

    public StudentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new StudentService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateStudent_WhenValidDataProvided()
    {
        // Arrange
        var studentForCreationDto = new StudentForCreationDto
        {
            Purpose = "Learn French"
        };

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.Insert(It.IsAny<StudentEntity>()));

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(studentForCreationDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentForCreationDto.Purpose, result.Purpose);
        Assert.NotEqual(Guid.Empty, result.Id); // Проверка, что Id был сгенерирован

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.Insert(It.IsAny<StudentEntity>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenStudentForCreationDtoIsNull()
    {
        // Arrange
        StudentForCreationDto studentForCreationDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _service.CreateAsync(studentForCreationDto, CancellationToken.None));

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.Insert(It.IsAny<StudentEntity>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotCallSaveChangesAsync_WhenStudentForCreationDtoIsNull()
    {
        // Arrange
        StudentForCreationDto studentForCreationDto = null;

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _service.CreateAsync(studentForCreationDto, CancellationToken.None));

        // Assert
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveStudent_WhenStudentExists()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new StudentEntity { Id = studentId, UserId = Guid.NewGuid()};

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(studentId, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.StudentRepository.Remove(student), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowStudentNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() => _service.DeleteAsync(studentId, CancellationToken.None));
        Assert.Equal($"The student with the identifier {studentId} was not found.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.Remove(It.IsAny<StudentEntity>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnStudentDtos_WhenStudentsExist()
    {
        // Arrange
        var students = new List<StudentEntity>
        {
            new() { Id = Guid.NewGuid(), Purpose = "Learn French", UserId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Purpose = "Learn Spanish", UserId = Guid.NewGuid() }
        };

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Purpose == "Learn French");
        Assert.Contains(result, s => s.Purpose == "Learn Spanish");

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoStudentsExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StudentEntity>());

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnStudentDto_WhenStudentExists()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var studentEntity = new StudentEntity { Id = studentId, Purpose = "Learn English", UserId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentEntity);

        // Act
        var result = await _service.GetByIdAsync(studentId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentId, result.Id);
        Assert.Equal("Learn English", result.Purpose);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowStudentNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() => _service.GetByIdAsync(studentId, CancellationToken.None));
        Assert.Equal($"The student with the identifier {studentId} was not found.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateStudent_WhenStudentExists()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var studentForUpdateDto = new StudentForUpdateDto { Purpose = "Learn German" };
        var existingStudent = new StudentEntity { Id = studentId, Purpose = "Learn French", UserId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStudent);

        // Act
        await _service.UpdateAsync(studentId, studentForUpdateDto, CancellationToken.None);

        // Assert
        Assert.Equal("Learn German", existingStudent.Purpose);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowStudentNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var studentForUpdateDto = new StudentForUpdateDto { Purpose = "Learn German" };

        _unitOfWorkMock.Setup(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<StudentNotFoundException>(() => _service.UpdateAsync(studentId, studentForUpdateDto, CancellationToken.None));
        Assert.Equal($"The student with the identifier {studentId} was not found.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.StudentRepository.GetByIdAsync(studentId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}