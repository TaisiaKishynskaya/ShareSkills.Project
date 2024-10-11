using App.Services.Abstract;
using Libraries.Contracts.Student;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public static class StudentEndpoint 
{
    public static void RegisterStudentEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/students",
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
                async (IStudentService service) =>
                {
                    var student = await service.GetAllAsync();

                    return Results.Ok(student);
                })
            .WithOpenApi();

        routeBuilder.MapGet("/students/{id:guid}", 
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
                async (Guid id, IStudentService service) =>
                {
                    var student = await service.GetByIdAsync(id);

                    return Results.Ok(student);
                })
            .WithOpenApi();

        routeBuilder.MapPost("/students", 
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}", Roles = UserRoles.Student)]
                async (StudentForCreationDto dto, IStudentService service) =>
                {
                    var student = await service.CreateAsync(dto);

                    return Results.Created($"/students/{student.Id}", student);
                })
            .WithOpenApi();

        routeBuilder.MapPut("/students/{id:guid}", 
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}", Roles = UserRoles.Student)]
                async (Guid id, StudentForUpdateDto dto, IStudentService service) =>
                {
                    await service.UpdateAsync(id, dto);

                    return Results.NoContent();
                })
            .WithOpenApi();

        routeBuilder.MapDelete("/students/{id:guid}", 
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}", Roles = UserRoles.Student)]
                async (Guid id, IStudentService service) =>
                {
                    await service.DeleteAsync(id);

                    return Results.NoContent();
                })
            .WithOpenApi();
    }
}