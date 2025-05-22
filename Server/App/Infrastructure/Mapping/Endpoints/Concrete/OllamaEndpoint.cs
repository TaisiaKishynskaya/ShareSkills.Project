using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Ollama;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class OllamaEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/ollama/generate",
                async ([FromBody] OllamaRequestDto dto, [FromServices] IOllamaService ollamaService) =>
                {
                    if (string.IsNullOrWhiteSpace(dto.Prompt))
                    {
                        return Results.BadRequest(new { message = "Prompt is required." });
                    }

                    var response = await ollamaService.GetOllamaResponseAsync(dto.Prompt);
                    return Results.Ok(new { response });
                })
            .WithName("GenerateOllamaResponse")
            .WithOpenApi();
    }
}
