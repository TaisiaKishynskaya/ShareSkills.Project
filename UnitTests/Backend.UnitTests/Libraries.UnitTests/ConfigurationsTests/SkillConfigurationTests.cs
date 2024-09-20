using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class SkillConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapSkillEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(SkillEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Skills", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForSkillEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(SkillEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurationsForSkillEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(SkillEntity));

            // Act
            var skillProperty = entityType.FindProperty("Skill");

            // Assert
            Assert.NotNull(skillProperty);
            Assert.False(skillProperty.IsNullable);
            Assert.Equal(100, skillProperty.GetMaxLength());
        }
    }

    [Fact]
    public void Configure_ShouldSetUpManyToManyRelationshipWithUsers()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var skillEntityType = context.Model.FindEntityType(typeof(SkillEntity));
            var userEntityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var skillToUserRelationship = skillEntityType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType == userEntityType);

            var userToSkillRelationship = userEntityType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType == skillEntityType);

            // Assert
            Assert.NotNull(skillToUserRelationship);
            Assert.NotNull(userToSkillRelationship);
        }
    }
}