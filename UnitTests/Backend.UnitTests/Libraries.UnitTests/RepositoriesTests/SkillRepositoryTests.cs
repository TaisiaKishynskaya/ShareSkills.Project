using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class SkillRepositoryTests
{
    private readonly SkillRepository _repository;
    private readonly AppDbContext _context;
    
    public SkillRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new SkillRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSkills()
    {
        // Arrange
        var skill1 = new SkillEntity { Id = Guid.NewGuid(), Skill = "Skill1" };
        var skill2 = new SkillEntity { Id = Guid.NewGuid(), Skill = "Skill2" };
        _context.Skills.AddRange(skill1, skill2);
        await _context.SaveChangesAsync();

        // Act
        var skills = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, skills.Count());
        Assert.Contains(skills, s => s.Skill == "Skill1");
        Assert.Contains(skills, s => s.Skill == "Skill2");
    }
    

    [Fact]
    public async Task GetTeacherSkillAsync_ShouldReturnCorrectSkill()
    {
        // Arrange
        var skill = new SkillEntity { Id = Guid.NewGuid(), Skill = "TeacherSkill" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherSkillAsync("TeacherSkill");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TeacherSkill", result?.Skill);
    }
    
    [Fact]
    public async Task GetTeacherSkillAsync_ShouldReturnNullIfSkillNameDoesNotExist()
    {
        // Arrange
        var skill = new SkillEntity { Id = Guid.NewGuid(), Skill = "ExistingSkill" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherSkillAsync("NonExistentSkill");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetTeacherSkillAsync_ShouldHandleCaseSensitivity()
    {
        // Arrange
        var skill = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillCaseSensitive" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherSkillAsync("skillcasesensitive");

        // Assert
        Assert.Null(result); // Assuming the search is case-sensitive
    }
    
    [Fact]
    public async Task GetTeacherSkillAsync_ShouldReturnCorrectSkillWhenMultipleSkillsExist()
    {
        // Arrange
        var skill1 = new SkillEntity { Id = Guid.NewGuid(), Skill = "Skill1" };
        var skill2 = new SkillEntity { Id = Guid.NewGuid(), Skill = "Skill2" };
        _context.Skills.AddRange(skill1, skill2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeacherSkillAsync("Skill2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Skill2", result?.Skill);
    }

    [Fact]
    public async Task GetTeacherSkillAsync_ShouldReturnNullWhenDatabaseIsEmpty()
    {
        // Act
        var result = await _repository.GetTeacherSkillAsync("AnySkill");

        // Assert
        Assert.Null(result);
    }
    

    [Fact]
    public async Task GetSkillAsync_ShouldReturnCorrectSkillById()
    {
        // Arrange
        var skill = new SkillEntity { Id = Guid.NewGuid(), Skill = "SkillById" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetSkillAsync(skill.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(skill.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetSkillAsync_ShouldReturnNullIfSkillIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetSkillAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }
}