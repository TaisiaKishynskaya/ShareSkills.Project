using Libraries.Data;
using Libraries.Data.UnitOfWork.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.DataTests;

public class UnitOfWorkTests
{
    private readonly UnitOfWork _unitOfWork;
    private readonly AppDbContext _context;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void UserRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.UserRepository);
        Assert.IsType<UserRepository>(_unitOfWork.UserRepository);
    }

    [Fact]
    public void StudentRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.StudentRepository);
        Assert.IsType<StudentRepository>(_unitOfWork.StudentRepository);
    }

    [Fact]
    public void TeacherRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.TeacherRepository);
        Assert.IsType<TeacherRepository>(_unitOfWork.TeacherRepository);
    }

    [Fact]
    public void MeetingRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.MeetingRepository);
        Assert.IsType<MeetingRepository>(_unitOfWork.MeetingRepository);
    }

    [Fact]
    public void GradeRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.GradeRepository);
        Assert.IsType<GradeRepository>(_unitOfWork.GradeRepository);
    }

    [Fact]
    public void SkillRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.SkillRepository);
        Assert.IsType<SkillRepository>(_unitOfWork.SkillRepository);
    }

    [Fact]
    public void LevelRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.LevelRepository);
        Assert.IsType<LevelRepository>(_unitOfWork.LevelRepository);
    }

    [Fact]
    public void ClassTimeRepository_ShouldBeInstantiated()
    {
        // Assert
        Assert.NotNull(_unitOfWork.ClassTimeRepository);
        Assert.IsType<ClassTimeRepository>(_unitOfWork.ClassTimeRepository);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = Guid.NewGuid() };
        _unitOfWork.TeacherRepository.Insert(teacher);

        // Act
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var savedTeacher = await _context.Teachers.FindAsync(teacher.Id);
        Assert.NotNull(savedTeacher);
        Assert.Equal(4.0, savedTeacher.Rating);
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => _context.Database.CanConnect());
    }
}
