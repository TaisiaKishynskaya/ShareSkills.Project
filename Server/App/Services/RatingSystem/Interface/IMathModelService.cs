using Libraries.Entities.Concrete;

namespace App.Services.RatingSystem.Interface;

public interface IMathModelService
{
    public Task Train();
    public float Predict(Guid userId, Guid courseId);
    public Task<IEnumerable<CourseEntity>> RecommendCourses(Guid userId, int topN = 5);
}