using App.Services.RecommendationSystem.Test;
using Libraries.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public static class RecommendationsEndpoint
{
    public static void RegisterRecommendationEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/rec-teachers", (AppDbContext dbContext, string userId) =>
            {
                var studentId = Guid.Parse(userId);

                var service = new TeacherRatingService(dbContext, new CosineSimilarityService());

                var recommendedTeachers = service.GetRecommendedTeachers(studentId, 3);

                var teachersIds = recommendedTeachers.Select(x =>x.Id).ToList();
                return Results.Ok(teachersIds);
            })
            .WithOpenApi();
        
        routeBuilder.MapGet("/rec-courses", (AppDbContext dbContext, string userId) =>
            {
                var studentId = Guid.Parse(userId);

                var service = new CourseRecommendationService(dbContext, new CosineSimilarityService());

                var recommendedTeachers = service.GetRecommendedCourses(studentId, 3);

                var teachersIds = recommendedTeachers.Select(x =>x.Name).ToList();
                return Results.Ok(teachersIds);
            })
            .WithOpenApi();
    }
}