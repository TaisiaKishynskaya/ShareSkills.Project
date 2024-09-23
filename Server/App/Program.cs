using App.Infrastructure.Configurations;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using Libraries.Data;
using Libraries.Entities.Concrete;

//TODO: Need refactoring, add validation for registration
namespace App;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        AuthenticationConfiguration.ConfigureAuthentication(builder);
        AuthorizationConfiguration.ConfigureAuthorization(builder); 

        // add identity and opt-in to endpoints
        builder.Services.AddIdentityCore<UserEntity>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        builder.Services.AddSwagger(builder.Configuration); 
        
        PolicyConfiguration.ConfigureCors(builder);
        
        DatabaseConfiguration.ConfigureDatabase(builder);
        
        ServicesConfiguration.ConfigureServices(builder);
        
        builder.Services.AddMinimalEndpoints();

        
        var app = builder.Build();
        
        app.UseCors("AllowAll");
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseHttpsRedirection();

        app.RegisterStudentEndpoint();
        app.RegisterMinimalEndpoints();

        AppConfiguration.ConfigureApp(app);
        
        DatabaseConfiguration.SeedDatabase(app);
    
        app.Run();
    }
}