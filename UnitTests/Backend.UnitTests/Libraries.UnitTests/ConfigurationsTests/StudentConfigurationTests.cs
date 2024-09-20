using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class StudentConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_ShouldMapToStudentsTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(StudentEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Students", tableName);
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

            var entityType = context.Model.FindEntityType(typeof(StudentEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveRequiredPropertyPurpose()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(StudentEntity));

            // Act
            var purposeProperty = entityType.FindProperty("Purpose");

            // Assert
            Assert.NotNull(purposeProperty);
            Assert.False(purposeProperty.IsNullable);
            Assert.Equal(200, purposeProperty.GetMaxLength());
        }
    }

    [Fact]
    public void Configure_ShouldSetUpManyToManyRelationshipWithGrades()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(StudentEntity));

            // Act
            var relationship = entityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Grades");

            // Assert
            Assert.NotNull(relationship);
            Assert.Equal("Grades", relationship.Name);

            var inverseNav = entityType.FindNavigation("Students");
            Assert.NotNull(inverseNav);
            Assert.Equal("Students", inverseNav.Name);
        }
    }
}