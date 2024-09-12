using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class MeetingConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapMeetingEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(MeetingEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Meetings", tableName);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPrimaryKeyForMeetingEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(MeetingEntity));

            // Act
            var primaryKey = entityType.FindPrimaryKey();

            // Assert
            Assert.NotNull(primaryKey);
            Assert.Equal("Id", primaryKey.Properties.First().Name);
        }
    }

    [Fact]
    public void Configure_ShouldHaveCorrectPropertyConfigurationsForMeetingEntity()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(MeetingEntity));

            // Act
            var ownerIdProperty = entityType.FindProperty("OwnerId");
            var foreignIdProperty = entityType.FindProperty("ForeignId");
            var dateTimeProperty = entityType.FindProperty("DateTime");
            var nameProperty = entityType.FindProperty("Name");
            var descriptionProperty = entityType.FindProperty("Description");

            // Assert
            Assert.NotNull(ownerIdProperty);
            Assert.False(ownerIdProperty.IsNullable);

            Assert.NotNull(foreignIdProperty);
            Assert.False(foreignIdProperty.IsNullable);

            Assert.NotNull(dateTimeProperty);
            Assert.False(dateTimeProperty.IsNullable);

            Assert.NotNull(nameProperty);
            Assert.False(nameProperty.IsNullable);

            Assert.NotNull(descriptionProperty);
        }
    }
}