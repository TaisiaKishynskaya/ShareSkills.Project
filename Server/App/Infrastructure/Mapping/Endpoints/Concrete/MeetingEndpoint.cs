using App.Infrastructure.Exceptions;
using App.Infrastructure.Exceptions.AlreadyExistsExceptions;
using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Meeting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class MeetingEndpoint  : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/meetings", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                async ([FromServices]IMeetingService servicem1) =>
            {
                var meeting = await servicem1.GetAllAsync();

                return Results.Ok(meeting);
            })
            .WithOpenApi();

        routeBuilder.MapGet("/meetings/{id:guid}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                async (Guid id, [FromServices]IMeetingService servicem2) =>
            {
                var meeting = await servicem2.GetByIdAsync(id);

                return Results.Ok(meeting);
            })
            .WithOpenApi();

        routeBuilder.MapPost("/meetings", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                async ([FromBody]MeetingForCreatingDto dto, [FromServices]IMeetingService servicem3) =>
            {
                try
                {
                    var meeting = await servicem3.TryToCreateAsync(dto);

                    return Results.Created($"/meetings/{meeting.Id}", meeting);
                }
                catch (MeetingAlreadyExistsException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithOpenApi();

        routeBuilder.MapPut("/meetings/{id:guid}", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                async (Guid id, [FromBody]MeetingForUpdateDto dto, [FromServices]IMeetingService servicem4) =>
            {
                await servicem4.UpdateAsync(id, dto);

                return Results.NoContent();
            })
            .WithOpenApi();

        routeBuilder.MapDelete("/meetings/{id:guid}", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                async (Guid id, [FromServices]IMeetingService servicem5) =>
            {
                await servicem5.DeleteAsync(id);

                return Results.NoContent();
            })
            .WithOpenApi();
    }
}