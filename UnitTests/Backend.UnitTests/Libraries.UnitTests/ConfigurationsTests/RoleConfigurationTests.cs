using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class RoleConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapRoleEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(RoleEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Roles", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForRoleEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(RoleEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurationsForRoleEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(RoleEntity));

            // Act
            var nameProperty = entityType.FindProperty("Name");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.False(nameProperty.IsNullable);
        }
    }

    [Fact]
    public void Configure_ShouldSetUpOneToManyRelationshipWithUsers()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var roleEntityType = context.Model.FindEntityType(typeof(RoleEntity));
            var userEntityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var roleToUserRelationship = roleEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Users");

            var userToRoleRelationship = userEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Role");

            // Assert
            Assert.NotNull(roleToUserRelationship);
            Assert.Equal("Users", roleToUserRelationship.Name);

            Assert.NotNull(userToRoleRelationship);
            Assert.Equal("Role", userToRoleRelationship.Name);
        }
    }
}