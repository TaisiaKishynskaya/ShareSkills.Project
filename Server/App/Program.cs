using App.Infrastructure.Configurations;
using App.Infrastructure.Mapping.Endpoints.Concrete;

//TODO: Need refactoring
namespace App;

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
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        //PolicyConfiguration.ConfigureCors(builder);

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
