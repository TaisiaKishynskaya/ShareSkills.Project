using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class GradeConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapGradeEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Grades", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForGradeEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurationForGradeEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var gradeProperty = entityType.FindProperty("Grade");

            // Assert
            Assert.NotNull(gradeProperty);
            Assert.Equal(1, gradeProperty.GetPrecision());
            Assert.Equal(0, gradeProperty.GetScale());
        }
    }

    [Fact]
    public void Configure_ShouldSetUpManyToManyRelationshipWithTeachers()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var relationship = entityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Teachers");

            // Assert
            Assert.NotNull(relationship);
            Assert.Equal("Teachers", relationship.Name);

            var inverseNav = entityType.FindNavigation("Grades");
            Assert.NotNull(inverseNav);
            Assert.Equal("Grades", inverseNav.Name);
        }
    }
}