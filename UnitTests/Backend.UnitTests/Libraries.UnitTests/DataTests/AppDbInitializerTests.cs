using Libraries.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Libraries.UnitTests.DataTests;

public class AppDbInitializerTests
{
    private readonly IServiceProvider _serviceProvider;

    public AppDbInitializerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        serviceCollection.AddScoped<AppDbInitializer>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void EnsureDatabaseCreated_ShouldCreateDatabase()
    {
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            AppDbInitializer.EnsureDatabaseCreated(_serviceProvider);

            // Assert
            Assert.True(context.Database.CanConnect());
        }
    }

    [Fact]
    public void EnsureDatabaseCreated_ShouldNotThrowExceptionWhenCalledMultipleTimes()
    {
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act & Assert
            var exception = Record.Exception(() => AppDbInitializer.EnsureDatabaseCreated(_serviceProvider));
            Assert.Null(exception);

            // Call again to ensure it still works without issues
            exception = Record.Exception(() => AppDbInitializer.EnsureDatabaseCreated(_serviceProvider));
            Assert.Null(exception);
        }
    }
}
