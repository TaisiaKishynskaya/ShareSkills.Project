using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Concrete;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class MLTeacherRecommendationEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/get-teacher", (Guid skillId, Guid levelId, Guid classTimeId, TeacherBinaryTree teacherTree) =>
            {
                var recommendedTeachers = teacherTree.Search(skillId, levelId, classTimeId);
                return Results.Ok(recommendedTeachers);
            })
            .WithOpenApi();
    }
}
