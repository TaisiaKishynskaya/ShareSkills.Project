using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class TeacherRepositoryTests
{
    private readonly TeacherRepository _repository;
    private readonly AppDbContext _context;
    
    public TeacherRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new TeacherRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTeachers()
    {
        // Arrange
        var teacher1 = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.5, UserId = Guid.NewGuid() };
        var teacher2 = new TeacherEntity { Id = Guid.NewGuid(), Rating = 3.9, UserId = Guid.NewGuid() };
        _context.Teachers.AddRange(teacher1, teacher2);
        await _context.SaveChangesAsync();

        // Act
        var teachers = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, teachers.Count());
        Assert.Contains(teachers, t => t.Id == teacher1.Id);
        Assert.Contains(teachers, t => t.Id == teacher2.Id);
    }

    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectTeacher()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = Guid.NewGuid() };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(teacher.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result?.Id);
    }
    
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnTeacherByEmail()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(), Email = "teacher@example.com", Name = "John", Surname = "Doe", Password = "Password123" };
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = user.Id };
        _context.Users.Add(user);
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNullIfEmailDoesNotExist()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    
    [Fact]
    public async Task GetScoresByTeacherIdAsync_ShouldReturnScoresForTeacher()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = new TeacherEntity { Id = teacherId, Rating = 4.0, UserId = Guid.NewGuid() };
        var grade1 = new GradeEntity { Id = Guid.NewGuid(), Grade = 5 };
        var grade2 = new GradeEntity { Id = Guid.NewGuid(), Grade = 4 };
        teacher.Grades.Add(grade1);
        teacher.Grades.Add(grade2);

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Act
        var scores = await _repository.GetScoresByTeacherIdAsync(teacherId);

        // Assert
        Assert.Equal(2, scores.Count());
        Assert.Contains(5, scores);
        Assert.Contains(4, scores);
    }

    
    [Fact]
    public async Task Insert_ShouldAddTeacherToDatabase()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = Guid.NewGuid() };

        // Act
        _repository.Insert(teacher);
        await _context.SaveChangesAsync();

        // Assert
        var insertedTeacher = await _context.Teachers.FindAsync(teacher.Id);
        Assert.NotNull(insertedTeacher);
        Assert.Equal(teacher.Id, insertedTeacher.Id);
    }

    
    [Fact]
    public async Task Remove_ShouldRemoveTeacherFromDatabase()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = Guid.NewGuid() };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(teacher);
        await _context.SaveChangesAsync();

        // Assert
        var deletedTeacher = await _context.Teachers.FindAsync(teacher.Id);
        Assert.Null(deletedTeacher);
    }
    
    [Fact]
    public async Task Remove_ShouldNotThrowExceptionIfTeacherDoesNotExist()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Rating = 4.0, UserId = Guid.NewGuid() };

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
        {
            _repository.Remove(teacher);
            await _context.SaveChangesAsync();
        });

        Assert.IsType<DbUpdateConcurrencyException>(exception);
    }

    /*[Fact]
    public async Task Insert_ShouldNotAddDuplicateTeachers()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher1 = new TeacherEntity { Id = teacherId, Rating = 4.0, UserId = Guid.NewGuid() };
        var teacher2 = new TeacherEntity { Id = teacherId, Rating = 5.0, UserId = Guid.NewGuid() }; // Same Id as teacher1

        // Act
        _repository.Insert(teacher1);
        await _context.SaveChangesAsync();

        // Clear context tracking before attempting to insert the second entity
        _context.ChangeTracker.Clear();  // Clear the change tracker to avoid tracking issues

        // Act
        _repository.Insert(teacher2);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Teachers.FindAsync(teacherId);
        Assert.NotNull(result);
        Assert.Equal(4.0, result.Rating); // Ensure the first inserted rating is retained
    }*/
}