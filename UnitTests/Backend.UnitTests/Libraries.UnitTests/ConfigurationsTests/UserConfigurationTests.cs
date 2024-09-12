using Microsoft.EntityFrameworkCore;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Xunit;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Libraries.UnitTests.ConfigurationsTests;

public class UserConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void SeedDatabase(AppDbContext context)
    {
        context.Users.Add(new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            RoleId = Guid.NewGuid()
        });

        context.SaveChanges();
    }

    [Fact]
    public void Configure_ShouldMapToUsersTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Users", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKey()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurations()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var nameProperty = entityType.FindProperty("Name");
            var surnameProperty = entityType.FindProperty("Surname");
            var emailProperty = entityType.FindProperty("Email");
            var passwordProperty = entityType.FindProperty("Password");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.Equal(30, nameProperty.GetMaxLength());
            Assert.False(nameProperty.IsNullable); // Property is required if it's not nullable

            Assert.NotNull(surnameProperty);
            Assert.Equal(30, surnameProperty.GetMaxLength());
            Assert.False(surnameProperty.IsNullable);

            Assert.NotNull(emailProperty);
            Assert.False(emailProperty.IsNullable);

            Assert.NotNull(passwordProperty);
            Assert.False(passwordProperty.IsNullable);
        }
    }

    [Fact]
    public void Configure_ShouldSetUpRelationships()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(UserEntity));

            // Act
            var roleNavigation = entityType.FindNavigation("Role");
            var studentNavigation = entityType.FindNavigation("Student");
            var teacherNavigation = entityType.FindNavigation("Teacher");

            // Assert
            Assert.NotNull(roleNavigation);
            Assert.Equal("Role", roleNavigation.Name);

            Assert.NotNull(studentNavigation);
            Assert.Equal("Student", studentNavigation.Name);

            Assert.NotNull(teacherNavigation);
            Assert.Equal("Teacher", teacherNavigation.Name);
        }
    }

    [Fact]
    public async Task Configure_ShouldCascadeDelete()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            SeedDatabase(context);

            var user = context.Users.First();
            var roleId = user.RoleId;

            context.Roles.Add(new RoleEntity { Id = roleId, Name = ""});
            await context.SaveChangesAsync();

            var role = await context.Roles.FindAsync(roleId);

            // Act
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            // Assert
            var deletedUser = await context.Users.FindAsync(user.Id);
            var deletedRole = await context.Roles.FindAsync(roleId);

            Assert.Null(deletedUser);
            Assert.NotNull(deletedRole);
        }
    }
}
