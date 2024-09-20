using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class ClassTimeRepositoryTests
{
    private readonly ClassTimeRepository _repository;
    private readonly AppDbContext _context;
    
    public ClassTimeRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new ClassTimeRepository(_context);
    }
    
    [Fact]
    public async Task GetTeacherClassTimeAsync_ShouldReturnClassTimeByName()
    {
        // Arrange
        var classTime = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Morning" };
        _context.ClassTimes.Add(classTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherClassTimeAsync("Morning");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Morning", result.Name);
    }

    [Fact]
    public async Task GetTeacherClassTimeAsync_ShouldReturnNullIfClassTimeNameDoesNotExist()
    {
        // Arrange
        var classTime = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Afternoon" };
        _context.ClassTimes.Add(classTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherClassTimeAsync("Evening");

        // Assert
        Assert.Null(result);
    }

    
    [Fact]
    public async Task GetClassTimeAsync_ShouldReturnClassTimeById()
    {
        // Arrange
        var classTime = new ClassTimeEntity { Id = Guid.NewGuid(), Name = "Evening" };
        _context.ClassTimes.Add(classTime);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetClassTimeAsync(classTime.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(classTime.Id, result.Id);
    }
    
    [Fact]
    public async Task GetTeacherClassTimeAsync_ShouldHandleNullName()
    {
        // Arrange
        // No class time added

        // Act
        var result = await _repository.GetTeacherClassTimeAsync(null);

        // Assert
        Assert.Null(result);
    }

    
    [Fact]
    public async Task GetClassTimeAsync_ShouldReturnNullIfClassTimeIdDoesNotExist()
    {
        // Arrange
        var classTimeId = Guid.NewGuid();

        // Act
        var result = await _repository.GetClassTimeAsync(classTimeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetClassTimeAsync_ShouldHandleNullId()
    {
        // Arrange
        // No class time added

        // Act
        var result = await _repository.GetClassTimeAsync(Guid.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetClassTimeAsync_ShouldHandleEmptyDatabase()
    {
        // Act
        var result = await _repository.GetClassTimeAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}