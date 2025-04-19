using App.Services.RatingSystem;
using App.Services.RatingSystem.Models;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;

public class CourseRecommender : Recommender
{
    private const string CoursesModelFileName = "CourseModel.zip";
    public CourseRecommender(AppDbContext dbContext) : base(dbContext, CoursesModelFileName) { }

    //TODO: этот метод использовать в случае, если вы хотите каждый раз пересчитывать модель. Это будет очень долго, поэтому я б рекомендовала этого избегать, если к данным очень частое обращение
    public void Train()
    {
        var inputs = _dbContext.Grades
            .SelectMany(g => g.Students.Select(u => new RatingInput
            {
                UserId = u.Id.ToString(),
                ItemId = g.Courses.First().Id.ToString(),
                Label = g.Grade
            }))
            .ToList();

        TrainModel(inputs);
    }
    
    //TODO: этот метод использовать в случае, если вы хотите 1 раз посчитать модель, а потом использовать уже готовую, сохраненную в файл, модель
    public void TrainAndSave()
    {
        var inputs = _dbContext.Grades
            .SelectMany(g => g.Students.Select(u => new RatingInput
            {
                UserId = u.Id.ToString(),
                ItemId = g.Courses.First().Id.ToString(),
                Label = g.Grade
            }))
            .ToList();

        TrainAndSave(inputs);
    }

    public async Task<IEnumerable<CourseEntity>> Recommend(Guid userId, int topN = 5)
    {
        var all = await _dbContext.Courses.ToListAsync();
        return all
            .Select(c => new { Entity = c, Score = PredictScore(new RatingInput { UserId = userId.ToString(), ItemId = c.Id.ToString() }) })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Entity);
    }
    
    public IEnumerable<CourseEntity> RecommendBySimilarity(Guid userId, int topN = 5) => RecommendationHelper.RecommendCoursesByUserSimilarity(_dbContext, userId, topN);
}