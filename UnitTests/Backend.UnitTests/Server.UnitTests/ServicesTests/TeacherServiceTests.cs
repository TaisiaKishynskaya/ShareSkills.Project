using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Contracts.Teacher;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class TeacherServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILevelService> _levelServiceMock;
    private readonly Mock<IClassTimeService> _classTimeServiceMock;
    private readonly Mock<ISkillService> _skillServiceMock;
    private readonly ITeacherService _service;
    
    public TeacherServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _levelServiceMock = new Mock<ILevelService>();
        _classTimeServiceMock = new Mock<IClassTimeService>();
        _skillServiceMock = new Mock<ISkillService>();

        _service = new TeacherService(_unitOfWorkMock.Object, _levelServiceMock.Object, _classTimeServiceMock.Object, _skillServiceMock.Object);
    }
    
    
    [Fact]
    public async Task CreateAsync_ShouldCreateTeacher_WhenValidDataProvided()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            Skill = "Math",
            Level = "Intermediate",
            ClassTime = "Morning (7:00 - 12:00)",
            Rating = 5,
            UserId = Guid.NewGuid()
        };

        var skillId = Guid.NewGuid();
        var levelId = Guid.NewGuid();
        var classTimeId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.SkillRepository.GetTeacherSkillAsync(teacherDto.Skill))
            .ReturnsAsync(new SkillEntity { Id = skillId });
        _unitOfWorkMock.Setup(uow => uow.LevelRepository.GetTeacherLevelAsync(teacherDto.Level))
            .ReturnsAsync(new LevelEntity { Id = levelId, Name = "Intermediate" });
        _unitOfWorkMock.Setup(uow => uow.ClassTimeRepository.GetTeacherClassTimeAsync(teacherDto.ClassTime))
            .ReturnsAsync(new ClassTimeEntity { Id = classTimeId, Name = "Morning (7:00 - 12:00)" });

        _levelServiceMock.Setup(ls => ls.GetLevelNameAsync(levelId))
            .ReturnsAsync("Intermediate");
        _classTimeServiceMock.Setup(cs => cs.GetClassTimeNameAsync(classTimeId))
            .ReturnsAsync("Morning (7:00 - 12:00)");
        _skillServiceMock.Setup(ss => ss.GetSkillNameAsync(skillId))
            .ReturnsAsync("Math");
        
        var cancellationToken = new CancellationToken(); // Create a CancellationToken

        // Act
        var result = await _service.CreateAsync(teacherDto, cancellationToken); // Pass the token

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacherDto.UserId, result.UserId);
        Assert.Equal(teacherDto.Rating, result.Rating);
        Assert.Equal("Intermediate", result.Level);
        Assert.Equal("Morning (7:00 - 12:00)", result.ClassTime);
        Assert.Equal("Math", result.Skill);

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.Insert(It.IsAny<TeacherEntity>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenSkillNotFound()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            Skill = "NonExistentSkill",
            Level = "Intermediate",
            ClassTime = "Morning (7:00 - 12:00)",
            Rating = 5,
            UserId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.SkillRepository.GetTeacherSkillAsync(teacherDto.Skill))
                       .ReturnsAsync((SkillEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(teacherDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenLevelNotFound()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            Skill = "Math",
            Level = "NonExistentLevel",
            ClassTime = "Morning (7:00 - 12:00)",
            Rating = 5,
            UserId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.SkillRepository.GetTeacherSkillAsync(teacherDto.Skill))
                       .ReturnsAsync(new SkillEntity { Id = Guid.NewGuid() });
        _unitOfWorkMock.Setup(uow => uow.LevelRepository.GetTeacherLevelAsync(teacherDto.Level))
                       .ReturnsAsync((LevelEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(teacherDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenClassTimeNotFound()
    {
        // Arrange
        var teacherDto = new TeacherForCreationDto
        {
            Skill = "Math",
            Level = "Intermediate",
            ClassTime = "NonExistentClassTime",
            Rating = 5,
            UserId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(uow => uow.SkillRepository.GetTeacherSkillAsync(teacherDto.Skill))
                       .ReturnsAsync(new SkillEntity { Id = Guid.NewGuid() });
        _unitOfWorkMock.Setup(uow => uow.LevelRepository.GetTeacherLevelAsync(teacherDto.Level))
                       .ReturnsAsync(new LevelEntity { Id = Guid.NewGuid(), Name = "Intermediate"  });
        _unitOfWorkMock.Setup(uow => uow.ClassTimeRepository.GetTeacherClassTimeAsync(teacherDto.ClassTime))
                       .ReturnsAsync((ClassTimeEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(teacherDto));
    }
    
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveTeacher_WhenTeacherExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = new TeacherEntity { Id = teacherId };

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        await _service.DeleteAsync(teacherId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.Remove(teacher), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowTeacherNotFoundException_WhenTeacherDoesNotExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeacherEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TeacherNotFoundException>(() => _service.DeleteAsync(teacherId));
        Assert.Contains(teacherId.ToString(), exception.Message); // Проверка, что сообщение содержит идентификатор
    }
    

    [Fact]
    public async Task GetAllAsync_ShouldReturnTeacherDtos_WhenTeachersExist()
    {
        // Arrange
        var teachers = new List<TeacherEntity>
        {
            new TeacherEntity 
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Rating = 5,
                LevelId = Guid.NewGuid(),
                ClassTimeId = Guid.NewGuid(),
                SkillId = Guid.NewGuid()
            }
        };

        var levelName = "Intermediate";
        var classTimeName = "Morning (7:00 - 12:00)";
        var skillName = "Math";

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(teachers);

        _levelServiceMock.Setup(ls => ls.GetLevelNameAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(levelName);
        _classTimeServiceMock.Setup(cs => cs.GetClassTimeNameAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(classTimeName);
        _skillServiceMock.Setup(ss => ss.GetSkillNameAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(skillName);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result); // Проверяем, что результат содержит один элемент
        var dto = result.First();
        Assert.Equal(teachers.First().Id, dto.Id);
        Assert.Equal(teachers.First().UserId, dto.UserId);
        Assert.Equal(teachers.First().Rating, dto.Rating);
        Assert.Equal(levelName, dto.Level);
        Assert.Equal(classTimeName, dto.ClassTime);
        Assert.Equal(skillName, dto.Skill);

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _levelServiceMock.Verify(ls => ls.GetLevelNameAsync(It.IsAny<Guid>()), Times.Once);
        _classTimeServiceMock.Verify(cs => cs.GetClassTimeNameAsync(It.IsAny<Guid>()), Times.Once);
        _skillServiceMock.Verify(ss => ss.GetSkillNameAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTeachersExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<TeacherEntity>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result); // Проверяем, что результат пуст
        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTeacherExtendedDto_WhenTeacherExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = new TeacherEntity 
        {
            Id = teacherId,
            UserId = Guid.NewGuid(),
            Rating = 5,
            LevelId = Guid.NewGuid(),
            ClassTimeId = Guid.NewGuid(),
            SkillId = Guid.NewGuid()
        };

        var user = new UserEntity 
        {
            Id = teacher.UserId,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com"
        };

        var levelName = "Intermediate";
        var classTimeName = "Morning (7:00 - 12:00)";
        var skillName = "Math";

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(teacher);
        _unitOfWorkMock.Setup(uow => uow.UserRepository.GetByIdAsync(teacher.UserId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
        _levelServiceMock.Setup(ls => ls.GetLevelNameAsync(teacher.LevelId))
                         .ReturnsAsync(levelName);
        _classTimeServiceMock.Setup(cs => cs.GetClassTimeNameAsync(teacher.ClassTimeId))
                             .ReturnsAsync(classTimeName);
        _skillServiceMock.Setup(ss => ss.GetSkillNameAsync(teacher.SkillId))
                         .ReturnsAsync(skillName);

        // Act
        var result = await _service.GetByIdAsync(teacherId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result.Id);
        Assert.Equal(teacher.UserId, result.UserId);
        Assert.Equal(teacher.Rating, result.Rating);
        Assert.Equal(levelName, result.Level);
        Assert.Equal(classTimeName, result.ClassTime);
        Assert.Equal(skillName, result.Skill);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Surname, result.Surname);
        Assert.Equal(user.Email, result.Email);

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.UserRepository.GetByIdAsync(teacher.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _levelServiceMock.Verify(ls => ls.GetLevelNameAsync(teacher.LevelId), Times.Once);
        _classTimeServiceMock.Verify(cs => cs.GetClassTimeNameAsync(teacher.ClassTimeId), Times.Once);
        _skillServiceMock.Verify(ss => ss.GetSkillNameAsync(teacher.SkillId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowTeacherNotFoundException_WhenTeacherDoesNotExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TeacherEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TeacherNotFoundException>(() => _service.GetByIdAsync(teacherId));
        Assert.Contains(teacherId.ToString(), exception.Message); // Проверка, что сообщение содержит идентификатор
        
        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.UserRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _levelServiceMock.Verify(ls => ls.GetLevelNameAsync(It.IsAny<Guid>()), Times.Never);
        _classTimeServiceMock.Verify(cs => cs.GetClassTimeNameAsync(It.IsAny<Guid>()), Times.Never);
        _skillServiceMock.Verify(ss => ss.GetSkillNameAsync(It.IsAny<Guid>()), Times.Never);
    }
    
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnTeacherId_WhenTeacherExists()
    {
        // Arrange
        var email = "teacher@example.com";
        var teacherId = Guid.NewGuid();
        var teacher = new TeacherEntity
        {
            Id = teacherId 
        };

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacherId, result);

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenTeacherDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeacherEntity)null);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    
    [Fact]
    public async Task RecountTotalGradeAsync_ShouldUpdateTeacherRating_WhenTeacherExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var newScore = 95;
        var teacher = new TeacherEntity
        {
            Id = teacherId,
            Rating = 50 // Старое значение
        };

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        await _service.RecountTotalGradeAsync(teacherId, newScore, CancellationToken.None);

        // Assert
        Assert.Equal(newScore, teacher.Rating); // Проверка, что рейтинг обновился

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task RecountTotalGradeAsync_ShouldThrowTeacherNotFoundException_WhenTeacherDoesNotExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var newScore = 95;

        _unitOfWorkMock.Setup(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeacherEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TeacherNotFoundException>(() => _service.RecountTotalGradeAsync(teacherId, newScore, CancellationToken.None));
        Assert.Contains(teacherId.ToString(), exception.Message); // Проверка, что сообщение содержит идентификатор

        _unitOfWorkMock.Verify(uow => uow.TeacherRepository.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); // SaveChangesAsync не должен быть вызван
    }
}