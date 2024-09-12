using Libraries.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Libraries.UnitTests.DataTests;

public class AppDbSeedTests
{
    private readonly IServiceProvider _serviceProvider;

    public AppDbSeedTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        serviceCollection.AddScoped<AppDbSeed>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Seed_ShouldAddInitialDataIfNotExists()
    {
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            AppDbSeed.Seed(context);

            // Assert
            Assert.True(context.Skills.Any(), "Skills were not seeded.");
            Assert.True(context.Roles.Any(), "Roles were not seeded.");
            Assert.True(context.ClassTimes.Any(), "ClassTimes were not seeded.");
            Assert.True(context.Levels.Any(), "Levels were not seeded.");

            // Additional assertions to check the data added
            Assert.Equal(5, context.Skills.Count());
            Assert.Equal(2, context.Roles.Count());
            Assert.Equal(3, context.ClassTimes.Count());
            Assert.Equal(3, context.Levels.Count());
        }
    }

    [Fact]
    public void Seed_ShouldNotAddDuplicateData()
    {
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Seed once
            AppDbSeed.Seed(context);

            // Act
            AppDbSeed.Seed(context); // Seed again

            // Assert
            var initialSkillsCount = context.Skills.Count();
            var initialRolesCount = context.Roles.Count();
            var initialClassTimesCount = context.ClassTimes.Count();
            var initialLevelsCount = context.Levels.Count();

            // Ensure no additional data was added
            Assert.Equal(5, initialSkillsCount);
            Assert.Equal(2, initialRolesCount);
            Assert.Equal(3, initialClassTimesCount);
            Assert.Equal(3, initialLevelsCount);
        }
    }
}
