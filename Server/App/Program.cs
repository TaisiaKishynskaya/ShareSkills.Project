using App;
using App.Infrastructure.Configurations;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Configurations;
using Libraries.Data;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Data.UnitOfWork.Concrete;
using Microsoft.EntityFrameworkCore;

// СМОТРЕТЬ https://github.com/TaisiaKishynskaya/CSharp_A-Level/tree/main/eShop.Project/Application/Catalog/Catalog.API

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --------------------------------------------------------------------------------------------------------------
        // ДОБАВЬТЕ ЭТОТ КОД когда пропишете классы с конфигурациями

        // AuthenticationConfiguration.ConfigureAuthentication(builder);
        // AuthorizationConfiguration.ConfigureAuthorization(builder); 

        builder.Services.AddSwagger(builder.Configuration); 

        /* не добавлять - черновик
        builder.Services.AddScoped<DbContext, AppDbContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IStudentService, StudentService>();
        builder.Services.AddScoped<ITeacherService, TeacherService>();*/
        
        DatabaseConfiguration.ConfigureDatabase(builder);
        ServicesConfiguration.ConfigureServices(builder);
        builder.Services.AddMinimalEndpoints();

        //----------------------------------------------------------------------------------------------------------------
        var app = builder.Build();
        
        app.UseHttpsRedirection();

        app.RegisterStudentEndpoint();

        app.RegisterMinimalEndpoints();

        AppConfiguration.ConfigureApp(app);
        
        app.Run();
    }
    
    // EF Core uses this method at design time to access the DbContext
    /*public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(
                            webBuilder => webBuilder.UseStartup<Startup>());*/
}
