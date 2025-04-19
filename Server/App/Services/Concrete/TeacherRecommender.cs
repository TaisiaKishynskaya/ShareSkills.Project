using App.Services.RatingSystem;
using App.Services.RatingSystem.Models;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;

public class TeacherRecommender : Recommender
{
    private const string TeachersModelFileName = "CourseModel.zip";
    public TeacherRecommender(AppDbContext dbContext) : base(dbContext, TeachersModelFileName) { }

    //TODO: этот метод использовать в случае, если вы хотите каждый раз пересчитывать модель. Это будет очень долго, поэтому я б рекомендовала этого избегать, если к данным очень частое обращение
    public void Train()
    {
        var inputs = _dbContext.Grades
            .SelectMany(g => g.Teachers.Select(t => new RatingInput
            {
                UserId = g.Students.First().Id.ToString(),
                ItemId = t.Id.ToString(),
                Label = g.Grade
            }))
            .ToList();

        TrainModel(inputs);
    }
    
    //TODO: этот метод использовать в случае, если вы хотите 1 раз посчитать модель, а потом использовать уже готовую, сохраненную в файл, модель.
    public void TrainAndSave()
    {
        var inputs = _dbContext.Grades
            .SelectMany(g => g.Teachers.Select(t => new RatingInput
            {
                UserId = g.Students.First().Id.ToString(),
                ItemId = t.Id.ToString(),
                Label = g.Grade
            }))
            .ToList();

        TrainAndSave(inputs);
    }

    public async Task<IEnumerable<TeacherEntity>> Recommend(Guid studentId, int topN = 5)
    {
        var all = await _dbContext.Teachers.ToListAsync();
        return all
            .Select(t => new { Entity = t, Score = PredictScore(new RatingInput { UserId = studentId.ToString(), ItemId = t.Id.ToString() }) })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Entity);
    }
    
    public IEnumerable<TeacherEntity> RecommendBySimilarity(Guid userId, int topN = 5) => RecommendationHelper.RecommendTeachersByUserSimilarity(_dbContext, userId, topN);
    public IEnumerable<TeacherEntity> RecommendByHistory(Guid userId, int topN = 5) => RecommendationHelper.RecommendTeachersByCourseHistory(_dbContext, userId, topN);
}