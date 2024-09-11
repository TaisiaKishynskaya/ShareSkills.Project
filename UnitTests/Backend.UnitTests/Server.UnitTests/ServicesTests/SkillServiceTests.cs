using App.Services.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class SkillServiceTests
{
    private readonly Mock<ISkillRepository> _skillRepositoryMock;
    private readonly SkillService _service;

    public SkillServiceTests()
    {
        _skillRepositoryMock = new Mock<ISkillRepository>();
        _service = new SkillService(_skillRepositoryMock.Object);
    }

    [Fact]
    public async Task AssignTeacherToSkillAsync_ShouldAssignSkill_WhenSkillExists()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Skill = null };
        var skillName = "Math";
        var skillEntity = new SkillEntity { Id = Guid.NewGuid(), Skill = skillName };

        _skillRepositoryMock.Setup(repo => repo.GetTeacherSkillAsync(skillName))
            .ReturnsAsync(skillEntity);

        // Act
        await _service.AssignTeacherToSkillAsync(teacher, skillName);

        // Assert
        Assert.NotNull(teacher.Skill);
        Assert.Equal(skillEntity, teacher.Skill);

        _skillRepositoryMock.Verify(repo => repo.GetTeacherSkillAsync(skillName), Times.Once);
    }

    [Fact]
    public async Task AssignTeacherToSkillAsync_ShouldThrowException_WhenSkillDoesNotExist()
    {
        // Arrange
        var teacher = new TeacherEntity { Id = Guid.NewGuid(), Skill = null };
        var skillName = "NonExistentSkill";

        _skillRepositoryMock.Setup(repo => repo.GetTeacherSkillAsync(skillName))
            .ReturnsAsync((SkillEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.AssignTeacherToSkillAsync(teacher, skillName));
        Assert.Equal("Skill doesn't exist", exception.Message);

        _skillRepositoryMock.Verify(repo => repo.GetTeacherSkillAsync(skillName), Times.Once);
    }
    
    
    [Fact]
    public async Task GetSkillNameAsync_ShouldReturnSkillName_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillName = "Math";
        var skillEntity = new SkillEntity { Id = skillId, Skill = skillName };

        _skillRepositoryMock.Setup(repo => repo.GetSkillAsync(skillId))
            .ReturnsAsync(skillEntity);

        // Act
        var result = await _service.GetSkillNameAsync(skillId);

        // Assert
        Assert.Equal(skillName, result);
        _skillRepositoryMock.Verify(repo => repo.GetSkillAsync(skillId), Times.Once);
    }

    [Fact]
    public async Task GetSkillNameAsync_ShouldThrowException_WhenSkillDoesNotExist()
    {
        // Arrange
        var skillId = Guid.NewGuid();

        _skillRepositoryMock.Setup(repo => repo.GetSkillAsync(skillId))
            .ReturnsAsync((SkillEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetSkillNameAsync(skillId));
        Assert.Equal("Skill doesn't exist", exception.Message);
        _skillRepositoryMock.Verify(repo => repo.GetSkillAsync(skillId), Times.Once);
    }
    
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSkills_WhenSkillsExist()
    {
        // Arrange
        var skills = new List<SkillEntity>
        {
            new SkillEntity { Id = Guid.NewGuid(), Skill = "Math" },
            new SkillEntity { Id = Guid.NewGuid(), Skill = "Science" }
        };

        _skillRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Skill == "Math");
        Assert.Contains(result, s => s.Skill == "Science");
        _skillRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoSkillsExist()
    {
        // Arrange
        _skillRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SkillEntity>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _skillRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}