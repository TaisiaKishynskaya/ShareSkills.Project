using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.ConfigurationsTests;

public class LevelConfigurationTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDatabaseOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
    
    [Fact]
    public void Configure_ShouldMapLevelEntityToTable()
    {
        // Arrange
        var options = GetInMemoryDatabaseOptions();

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var entityType = context.Model.FindEntityType(typeof(LevelEntity));

            // Act
            var tableName = entityType.GetTableName();

            // Assert
            Assert.Equal("Levels", tableName);
        }
    }

}