using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class GradeRepositoryTests
{
    private readonly GradeRepository _repository;
    private readonly AppDbContext _context;
    
    public GradeRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new GradeRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGrades()
    {
        // Arrange
        var grade1 = new GradeEntity { Id = Guid.NewGuid(), Grade = 4 };
        var grade2 = new GradeEntity { Id = Guid.NewGuid(), Grade = 5 };
        _context.Grades.AddRange(grade1, grade2);
        await _context.SaveChangesAsync();

        // Act
        var grades = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, grades.Count());
        Assert.Contains(grades, g => g.Grade == 4);
        Assert.Contains(grades, g => g.Grade == 5);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnGradeById()
    {
        // Arrange
        var grade = new GradeEntity { Id = Guid.NewGuid(), Grade = 3 };
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(grade.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(grade.Id, result?.Id);
    }

    [Fact]
    public async Task Insert_ShouldAddNewGrade()
    {
        // Arrange
        var grade = new GradeEntity { Id = Guid.NewGuid(), Grade = 2 };

        // Act
        _repository.Insert(grade);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Grades.FindAsync(grade.Id);
        Assert.NotNull(result);
        Assert.Equal(grade.Grade, result.Grade);
    }

    
    [Fact]
    public async Task Remove_ShouldDeleteGrade()
    {
        // Arrange
        var grade = new GradeEntity { Id = Guid.NewGuid(), Grade = 1 };
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(grade);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Grades.FindAsync(grade.Id);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Remove_ShouldThrowExceptionIfGradeDoesNotExist()
    {
        // Arrange
        var grade = new GradeEntity { Id = Guid.NewGuid(), Grade = 99 };

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
        {
            _repository.Remove(grade);
            await _context.SaveChangesAsync();
        });

        Assert.IsType<DbUpdateConcurrencyException>(exception);
    }
}