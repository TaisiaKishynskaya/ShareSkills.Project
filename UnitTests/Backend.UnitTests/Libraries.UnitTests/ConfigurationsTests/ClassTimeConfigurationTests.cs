using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class ClassTimeConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapClassTimeEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(ClassTimeEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("ClassTimes", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForClassTimeEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(ClassTimeEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveRequiredNamePropertyForClassTimeEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(ClassTimeEntity));

            // Act
            var nameProperty = entityType.FindProperty("Name");

            // Assert
            Assert.NotNull(nameProperty);
            Assert.False(nameProperty.IsNullable);
        }
    }

    [Fact]
    public void Configure_ShouldSetUpOneToManyRelationshipWithTeachers()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(ClassTimeEntity));

            // Act
            var relationship = entityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Teachers");

            // Assert
            Assert.NotNull(relationship);
            Assert.Equal("Teachers", relationship.Name);

            var inverseNav = entityType.FindNavigation("ClassTime");
            //Assert.NotNull(inverseNav);
            Assert.Equal("ClassTime", inverseNav?.Name);
        }
    }
}