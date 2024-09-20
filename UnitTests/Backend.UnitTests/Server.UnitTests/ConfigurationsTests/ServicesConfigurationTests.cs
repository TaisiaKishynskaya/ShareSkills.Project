using App.Infrastructure.Configurations;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Data;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Data.UnitOfWork.Concrete;
using Libraries.Repositories.Abstract;
using Libraries.Repositories.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Server.UnitTests.ConfigurationsTests;

public class ServicesConfigurationTests
{
    [Fact]
    public void ConfigureServices_ShouldRegisterAllServices()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Register an in-memory database for testing
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));
        
        // Act
        ServicesConfiguration.ConfigureServices(builder);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IUnitOfWork>());
        Assert.NotNull(serviceProvider.GetService<IUserRepository>());
        Assert.NotNull(serviceProvider.GetService<IUserService>());
        Assert.NotNull(serviceProvider.GetService<IStudentRepository>());
        Assert.NotNull(serviceProvider.GetService<IStudentService>());
        Assert.NotNull(serviceProvider.GetService<ITeacherRepository>());
        Assert.NotNull(serviceProvider.GetService<ITeacherService>());
        Assert.NotNull(serviceProvider.GetService<IRoleRepository>());
        Assert.NotNull(serviceProvider.GetService<IRoleService>());
        Assert.NotNull(serviceProvider.GetService<ISkillRepository>());
        Assert.NotNull(serviceProvider.GetService<ISkillService>());
        Assert.NotNull(serviceProvider.GetService<IMeetingRepository>());
        Assert.NotNull(serviceProvider.GetService<IMeetingService>());
        Assert.NotNull(serviceProvider.GetService<IGradeRepository>());
        Assert.NotNull(serviceProvider.GetService<IGradeService>());
        Assert.NotNull(serviceProvider.GetService<IClassTimeRepository>());
        Assert.NotNull(serviceProvider.GetService<IClassTimeService>());
        Assert.NotNull(serviceProvider.GetService<ILevelRepository>());
        Assert.NotNull(serviceProvider.GetService<ILevelService>());
        Assert.NotNull(serviceProvider.GetService<TeacherBinaryTree>());

        // Validate that services are registered with the correct lifetime
        Assert.IsType<UnitOfWork>(serviceProvider.GetService<IUnitOfWork>());
        Assert.IsType<UserRepository>(serviceProvider.GetService<IUserRepository>());
        Assert.IsType<UserService>(serviceProvider.GetService<IUserService>());
        Assert.IsType<StudentRepository>(serviceProvider.GetService<IStudentRepository>());
        Assert.IsType<StudentService>(serviceProvider.GetService<IStudentService>());
        Assert.IsType<TeacherRepository>(serviceProvider.GetService<ITeacherRepository>());
        Assert.IsType<TeacherService>(serviceProvider.GetService<ITeacherService>());
        Assert.IsType<RoleRepository>(serviceProvider.GetService<IRoleRepository>());
        Assert.IsType<RoleService>(serviceProvider.GetService<IRoleService>());
        Assert.IsType<SkillRepository>(serviceProvider.GetService<ISkillRepository>());
        Assert.IsType<SkillService>(serviceProvider.GetService<ISkillService>());
        Assert.IsType<MeetingRepository>(serviceProvider.GetService<IMeetingRepository>());
        Assert.IsType<MeetingService>(serviceProvider.GetService<IMeetingService>());
        Assert.IsType<GradeRepository>(serviceProvider.GetService<IGradeRepository>());
        Assert.IsType<GradeService>(serviceProvider.GetService<IGradeService>());
        Assert.IsType<ClassTimeRepository>(serviceProvider.GetService<IClassTimeRepository>());
        Assert.IsType<ClassTimeService>(serviceProvider.GetService<IClassTimeService>());
        Assert.IsType<LevelRepository>(serviceProvider.GetService<ILevelRepository>());
        Assert.IsType<LevelService>(serviceProvider.GetService<ILevelService>());
        Assert.IsType<TeacherBinaryTree>(serviceProvider.GetService<TeacherBinaryTree>());
    }
}