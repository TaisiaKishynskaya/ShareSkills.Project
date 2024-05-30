using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Data.UnitOfWork.Concrete;
using Libraries.Repositories.Abstract;
using Libraries.Repositories.Concrete;

namespace App.Infrastructure.Configurations;

public static class ServicesConfiguration
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<IStudentRepository, StudentRepository>();
        builder.Services.AddScoped<IStudentService, StudentService>();

        builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
        builder.Services.AddScoped<ITeacherService, TeacherService>();
        
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IRoleService, RoleService>();

        builder.Services.AddScoped<ISkillRepository, SkillRepository>();
        // Не работает: Unable to resolve service for type 'Libraries.Data.UnitOfWork.Concrete.UnitOfWork' while attempting to activate 'App.Services.Concrete.SkillService'
        // builder.Services.AddScoped<ISkillService, SkillService>();

        builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
        builder.Services.AddScoped<IMeetingService, MeetingService>();

        //builder.Services.AddAutoMapper();

        //builder.Services.AddFluentValidationAutoValidation();
        //builder.Services.AddFluentValidationClientsideAdapters();

        //builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        builder.Services.AddEndpointsApiExplorer(); 
    }
}