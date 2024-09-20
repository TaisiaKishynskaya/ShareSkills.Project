using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class LevelRepositoryTests
{
    private readonly LevelRepository _repository;
    private readonly AppDbContext _context;
    
    public LevelRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new LevelRepository(_context);
    }
    
    [Fact]
    public async Task GetTeacherLevelAsync_ShouldReturnCorrectLevel()
    {
        // Arrange
        var level = new LevelEntity { Id = Guid.NewGuid(), Name = "Advanced" };
        _context.Levels.Add(level);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherLevelAsync("Advanced");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced", result?.Name);
    }
    
    [Fact]
    public async Task GetTeacherLevelAsync_ShouldReturnNullIfLevelNameDoesNotExist()
    {
        // Arrange
        var level = new LevelEntity { Id = Guid.NewGuid(), Name = "Beginner" };
        _context.Levels.Add(level);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherLevelAsync("NonExistentLevel");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetTeacherLevelAsync_ShouldHandleCaseSensitivity()
    {
        // Arrange
        var level = new LevelEntity { Id = Guid.NewGuid(), Name = "Expert" };
        _context.Levels.Add(level);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherLevelAsync("expert");

        // Assert
        Assert.Null(result); // Assuming the search is case-sensitive
    }

    [Fact]
    public async Task GetTeacherLevelAsync_ShouldReturnCorrectLevelWhenMultipleLevelsExist()
    {
        // Arrange
        var level1 = new LevelEntity { Id = Guid.NewGuid(), Name = "Beginner" };
        var level2 = new LevelEntity { Id = Guid.NewGuid(), Name = "Advanced" };
        _context.Levels.AddRange(level1, level2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherLevelAsync("Advanced");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced", result?.Name);
    }

    [Fact]
    public async Task GetTeacherLevelAsync_ShouldReturnNullWhenDatabaseIsEmpty()
    {
        // Act
        var result = await _repository.GetTeacherLevelAsync("AnyLevel");

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetLevelAsync_ShouldReturnLevelById()
    {
        // Arrange
        var level = new LevelEntity { Id = Guid.NewGuid(), Name = "Intermediate" };
        _context.Levels.Add(level);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLevelAsync(level.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(level.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetLevelAsync_ShouldReturnNullIfLevelIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetLevelAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }
}