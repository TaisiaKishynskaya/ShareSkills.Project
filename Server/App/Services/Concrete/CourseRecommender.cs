using App.Services.RatingSystem;
using App.Services.RatingSystem.Models;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;

public class CourseRecommender : Recommender
{
    public CourseRecommender(AppDbContext dbContext) : base(dbContext) { }

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

    public async Task<IEnumerable<CourseEntity>> Recommend(Guid userId, int topN = 5)
    {
        var all = await _dbContext.Courses.ToListAsync();
        return all
            .Select(c => new { Entity = c, Score = PredictScore(new RatingInput { UserId = userId.ToString(), ItemId = c.Id.ToString() }) })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Entity);
    }
}