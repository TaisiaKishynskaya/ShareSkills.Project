// using App.Infrastructure.Mapping.Endpoints.Abstract;
// using App.Services.Abstract;
// using Libraries.Contracts.Skill;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Guid = System.Guid;
//
// namespace App.Infrastructure.Mapping.Endpoints.Concrete;
//
// public class SkillEndpoint : IMinimalEndpoint
// {
//     public void MapRoutes(IEndpointRouteBuilder routeBuilder)
//     {
//         routeBuilder.MapGet("/skills", 
//                 [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//                 async ([FromServices]ISkillService service) =>
//                 {
//                     var skill = await service.GetAllAsync();
//
//                     return Results.Ok(skill);
//                 })
//             .WithOpenApi();
//
//         routeBuilder.MapGet("/skills/{id:guid}",
//                 [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//                 async (Guid id, [FromServices]ISkillService service) =>
//                 {
//                     var skill = await service.GetByIdAsync(id);
//
//                     return Results.Ok(skill);
//                 })
//             .WithOpenApi();
//
//         routeBuilder.MapPost("/skills", 
//                 [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//                 async ([FromBody]SkillForCreatingDto dto, [FromServices]ISkillService service) =>
//                 {
//                     var skill = await service.CreateAsync(dto);
//
//                     return Results.Created($"/skills/{skill.Id}", skill);
//                 })
//             .WithOpenApi();
//
//         routeBuilder.MapPut("/skills/{id:guid}", 
//                 [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//                 async (Guid id, [FromBody]SkillForUpdateDto dto, [FromServices]ISkillService service) =>
//                 {
//                     await service.UpdateAsync(id, dto);
//
//                     return Results.NoContent();
//                 })
//             .WithOpenApi();
//
//         routeBuilder.MapDelete("/skills/{id:guid}", 
//                 [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//                 async (Guid id, [FromServices]ISkillService service) =>
//                 {
//                     await service.DeleteAsync(id);
//
//                     return Results.NoContent();
//                 })
//             .WithOpenApi();
//     }
// }