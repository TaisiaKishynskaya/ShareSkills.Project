using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Mapping.Endpoints.Concrete
{
    public class MLTeacherRecommendationEndpoint : IMinimalEndpoint
    {
        public void MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/get-teacher", async (HttpContext context, TeacherBinaryTree teacherTree) =>
                {
                    // Получаем параметры запроса из URL
                    var skillId = context.Request.Query["skillId"];
                    var levelId = context.Request.Query["levelId"];
                    var classTimeId = context.Request.Query["classTimeId"];

                    // Преобразуем параметры из строки в Guid
                    if (!Guid.TryParse(skillId, out var skillGuid) || 
                        !Guid.TryParse(levelId, out var levelGuid) || 
                        !Guid.TryParse(classTimeId, out var classTimeGuid))
                    {
                        return Results.BadRequest("Invalid parameters");
                    }

                    // Выполняем поиск учителя с помощью бинарного дерева
                    var recommendedTeachers = teacherTree.Search(skillGuid, levelGuid, classTimeGuid);
                
                    // Возвращаем результат
                    return Results.Ok(recommendedTeachers);
                })
                .WithOpenApi();
        }
    }
}