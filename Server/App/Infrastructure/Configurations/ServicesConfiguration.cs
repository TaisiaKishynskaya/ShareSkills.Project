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
        builder.Services.AddScoped<ISkillService, SkillService>();
        
        builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
        builder.Services.AddScoped<IMeetingService, MeetingService>();
        
        builder.Services.AddScoped<IGradeRepository, GradeRepository>();
        builder.Services.AddScoped<IGradeService, GradeService>();
        
        builder.Services.AddScoped<IClassTimeRepository, ClassTimeRepository>();
        builder.Services.AddScoped<IClassTimeService, ClassTimeService>();
        
        builder.Services.AddScoped<ILevelRepository, LevelRepository>();
        builder.Services.AddScoped<ILevelService, LevelService>();

        //builder.Services.AddAutoMapper();

        //builder.Services.AddFluentValidationAutoValidation();
        //builder.Services.AddFluentValidationClientsideAdapters();

        //builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        builder.Services.AddEndpointsApiExplorer(); 
    }
}