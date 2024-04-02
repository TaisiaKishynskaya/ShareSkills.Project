namespace App.Data;

public class AppDbInitializer
{
    public static void EnsureDatabaseCreated(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }
    }
}