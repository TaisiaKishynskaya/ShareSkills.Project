using App.Services.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class RecommendationService : IRecommendationService
{
    private readonly CourseRecommender _courseRec;
    private readonly TeacherRecommender _teacherRec;

    public RecommendationService(CourseRecommender courseRec, TeacherRecommender teacherRec)
    {
        _courseRec = courseRec;
        _teacherRec = teacherRec;
    }

    public Task<IEnumerable<CourseEntity>> GetRecommendedCourses(Guid userId, int count = 5)
    {
        _courseRec.Train();
        return _courseRec.Recommend(userId, count);
    }

    public Task<IEnumerable<TeacherEntity>> RecommendTopTeachers(Guid userId, int count = 5)
    {
        _teacherRec.Train();
        return _teacherRec.Recommend(userId, count);
    }
}
