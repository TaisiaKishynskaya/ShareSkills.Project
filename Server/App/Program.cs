using App.Infrastructure.Configurations;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using Libraries.Data;

// СМОТРЕТЬ https://github.com/TaisiaKishynskaya/CSharp_A-Level/tree/main/eShop.Project/Application/Catalog/Catalog.API
//TODO: Need refactoring, add validation for registration
internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AuthenticationConfiguration.ConfigureAuthentication(builder);
        AuthorizationConfiguration.ConfigureAuthorization(builder); 

        builder.Services.AddSwagger(builder.Configuration); 
        builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });
        
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
