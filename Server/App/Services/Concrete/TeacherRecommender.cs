using App.Services.RatingSystem;
using App.Services.RatingSystem.Models;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;

public class TeacherRecommender : Recommender
{
    public TeacherRecommender(AppDbContext dbContext) : base(dbContext) { }

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

    public async Task<IEnumerable<TeacherEntity>> Recommend(Guid studentId, int topN = 5)
    {
        var all = await _dbContext.Teachers.ToListAsync();
        return all
            .Select(t => new { Entity = t, Score = PredictScore(new RatingInput { UserId = studentId.ToString(), ItemId = t.Id.ToString() }) })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Entity);
    }
}