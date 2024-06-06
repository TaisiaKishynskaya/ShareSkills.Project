using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Concrete;
using Libraries.Data.UnitOfWork.Abstract;

namespace App.Infrastructure.Mapping.Endpoints.Concrete
{
    public class MLTeacherRecommendationEndpoint : IMinimalEndpoint
    {
        public void MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/get-teacher", async (string skill, string level, string classTime, IUnitOfWork unitOfWork, ILogger<TeacherBinaryTree> treeLogger, ILogger<MLTeacherRecommendationEndpoint> logger) =>
            {
                var skillEntity = await unitOfWork.SkillRepository.GetTeacherSkillAsync(skill);
                if (skillEntity == null)
                {
                    logger.LogWarning("Skill not found: {Skill}", skill);
                    return Results.BadRequest(new { StatusCode = 400, Message = $"Skill '{skill}' not found." });
                }

                var levelEntity = await unitOfWork.LevelRepository.GetTeacherLevelAsync(level);
                if (levelEntity == null)
                {
                    logger.LogWarning("Level not found: {Level}", level);
                    return Results.BadRequest(new { StatusCode = 400, Message = $"Level '{level}' not found." });
                }

                var classTimeEntity = await unitOfWork.ClassTimeRepository.GetTeacherClassTimeAsync(classTime);
                if (classTimeEntity == null)
                {
                    logger.LogWarning("ClassTime not found: {ClassTime}", classTime);
                    return Results.BadRequest(new { StatusCode = 400, Message = $"ClassTime '{classTime}' not found." });
                }

                var skillId = skillEntity.Id;
                var levelId = levelEntity.Id;
                var classTimeId = classTimeEntity.Id;

                var teacherTree = new TeacherBinaryTree(treeLogger, unitOfWork);
                var recommendedTeacher = teacherTree.Search(skillId, levelId, classTimeId);

                if (recommendedTeacher == null)
                {
                    return Results.NotFound(new { StatusCode = 404, Message = "Teacher not found." });
                }

                // Возвращаем только идентификатор найденного преподавателя
                return Results.Ok(recommendedTeacher.Id.ToString());
            })
            .WithOpenApi();
        }
    }
}
