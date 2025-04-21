using App.Services.RecommendationSystem.Test;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public static class RecommendationsEndpoint
{
    public static void RegisterRecommendationEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        /*routeBuilder.MapGet("/rec",
                //[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
                async () =>
                {
                    var fakeData = new FakeAppDbContext();
                    var studentId = fakeData.Students.First().Id;
                    var service = new TeacherRatingService(fakeData);

                    // Здесь обязательно сохраняем результат в переменную
                    var recs = service.GetRecommendedTeachers(studentId, 2);

                    // И возвращаем именно его
                    return Results.Ok(recs);
                })
            .WithOpenApi();*/
        
        
        routeBuilder.MapGet("/rec", () =>
            {
                // Инициализация фейковых данных
                var context = new FakeAppDbContext();

                // Студент, которого мы явно создавали
                var studentId = Guid.Parse("99999999-9999-9999-9999-999999999999");

                // Инициализация сервиса рекомендаций
                var service = new TeacherRatingService(context);

                // Получение топ-3 рекомендованных преподавателей
                var recommendedTeachers = service.GetRecommendedTeachers(studentId, 3);

                return Results.Ok(recommendedTeachers);
            })
            .WithOpenApi();
    }
}