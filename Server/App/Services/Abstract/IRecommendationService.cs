using Libraries.Entities.Concrete;

namespace App.Services.Abstract;

public interface IRecommendationService
{
    public Task<IEnumerable<TeacherEntity>> RecommendTopTeachers(Guid userId, int count = 5);
}