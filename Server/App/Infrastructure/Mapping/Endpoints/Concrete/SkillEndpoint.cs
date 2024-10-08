using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Skill;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Guid = System.Guid;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class SkillEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/skills", 
                [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
                async ([FromServices]ISkillService service) => 
                {
                    var skill = await service.GetAllAsync();

                    return Results.Ok(skill);
                })
            .WithOpenApi();
    }
}