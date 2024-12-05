using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class ReportEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/reports/learning/{userId:guid}",
            async (Guid userId, [FromServices] IReportService reportService) =>
            {
                var report = await reportService.GetReportForUserAsync(userId);
                return report.Any() ? Results.Ok(report) : Results.NotFound(new { Message = "No data found for the specified user." });
            });
    }
}