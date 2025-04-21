using Libraries.Entities.Concrete;

namespace App.Services.RecommendationSystem.Abstract;

public interface ITeacherRecommendationService
{
    public void Train();
    public List<(TeacherEntity Teacher, float Score)> GetRecommendations(Guid studentId, int topN = 5);
}