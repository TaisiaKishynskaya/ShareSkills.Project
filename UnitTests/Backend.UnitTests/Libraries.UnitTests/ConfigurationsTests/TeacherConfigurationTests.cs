using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class TeacherConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldSetUpManyToManyRelationshipWithGrades()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var teacherEntityType = context.Model.FindEntityType(typeof(TeacherEntity));
            var gradeEntityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var teacherToGradeNavigation = teacherEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Grades");
            var gradeToTeacherNavigation = gradeEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Teachers");

            // Assert
            Assert.NotNull(teacherToGradeNavigation);
            Assert.Equal("Grades", teacherToGradeNavigation.Name);

            Assert.NotNull(gradeToTeacherNavigation);
            Assert.Equal("Teachers", gradeToTeacherNavigation.Name);
        }
    }
    
    
    [Fact]
    public void Configure_ShouldMapTeacherEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(TeacherEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Teachers", tableName);
        }
    }

    
    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForTeacherEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(TeacherEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    
    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurationsForTeacherEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(TeacherEntity));

            // Act
            var ratingProperty = entityType.FindProperty("Rating");

            // Assert
            Assert.NotNull(ratingProperty);
            Assert.False(ratingProperty.IsNullable);
            Assert.Equal(2, ratingProperty.GetPrecision());
            Assert.Equal(1, ratingProperty.GetScale());
        }
    }

    
    [Fact]
    public void Configure_GradeEntity_ShouldMapToTeachersAndStudents()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var gradeEntityType = context.Model.FindEntityType(typeof(GradeEntity));

            // Act
            var teacherNavigation = gradeEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Teachers");
            var studentNavigation = gradeEntityType.GetNavigations()
                .FirstOrDefault(nav => nav.Name == "Students");

            // Assert
            Assert.NotNull(teacherNavigation);
            Assert.Equal("Teachers", teacherNavigation.Name);

            Assert.NotNull(studentNavigation);
            Assert.Equal("Students", studentNavigation.Name);
        }
    }
}