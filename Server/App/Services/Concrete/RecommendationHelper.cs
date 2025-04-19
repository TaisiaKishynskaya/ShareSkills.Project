using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;

public class RecommendationHelper
{
        public static IEnumerable<CourseEntity> RecommendCoursesByUserSimilarity(AppDbContext dbContext, Guid userId, int topN = 5)
        {
            var allCourses = dbContext.Courses.ToList();
            var users = dbContext.Users.ToList();
            var courseIds = allCourses.Select(c => c.Id).ToList();

            // Матриця рейтингів: User -> вектор по курсах
            var userVectors = users.ToDictionary(
                u => u.Id,
                u => {
                    var vec = new double[courseIds.Count];
                    var grades = dbContext.Grades
                        .Where(g => g.Students.Any(s => s.Id == u.Id))
                        .ToList();
                    foreach (var g in grades)
                    {
                        var idx = courseIds.IndexOf(g.Courses.First().Id);
                        if (idx >= 0) vec[idx] = g.Grade;
                    }
                    return vec;
                });

            var targetVec = userVectors[userId];
            // Обчислення косинусної схожості
            var sims = userVectors
                .Where(kv => kv.Key != userId)
                .Select(kv => new { UserId = kv.Key, Sim = MathsMethods.CosineSimilarity(targetVec, kv.Value) })
                .OrderByDescending(x => x.Sim)
                .Take(3)
                .ToList();

            var scores = new Dictionary<Guid, double>();
            // Зважені оцінки від схожих
            foreach (var s in sims)
            {
                var vec = userVectors[s.UserId];
                for (int i = 0; i < courseIds.Count; i++)
                {
                    if (targetVec[i] == 0 && vec[i] > 0)
                        scores[courseIds[i]] = scores.GetValueOrDefault(courseIds[i]) + vec[i] * s.Sim;
                }
            }

            return scores
                .OrderByDescending(kv => kv.Value)
                .Take(topN)
                .Select(kv => allCourses.First(c => c.Id == kv.Key));
        }

        public static IEnumerable<TeacherEntity> RecommendTeachersByUserSimilarity(AppDbContext dbContext, Guid userId, int topN = 5)
        {
            var allTeachers = dbContext.Teachers.ToList();
            var users = dbContext.Users.ToList();
            var teacherIds = allTeachers.Select(t => t.Id).ToList();

            // Матриця рейтингів: User -> вектор по викладачах
            var userVectors = users.ToDictionary(
                u => u.Id,
                u => {
                    var vec = new double[teacherIds.Count];
                    var grades = dbContext.Grades
                        .Where(g => g.Students.Any(s => s.Id == u.Id))
                        .ToList();
                    foreach (var g in grades)
                    {
                        foreach (var t in g.Teachers)
                        {
                            var idx = teacherIds.IndexOf(t.Id);
                            if (idx >= 0) vec[idx] = g.Grade;
                        }
                    }
                    return vec;
                });

            var targetVec = userVectors[userId];
            var sims = userVectors
                .Where(kv => kv.Key != userId)
                .Select(kv => new { UserId = kv.Key, Sim = MathsMethods.CosineSimilarity(targetVec, kv.Value) })
                .OrderByDescending(x => x.Sim)
                .Take(3)
                .ToList();

            var scores = new Dictionary<Guid, double>();
            foreach (var s in sims)
            {
                var vec = userVectors[s.UserId];
                for (int i = 0; i < teacherIds.Count; i++)
                {
                    if (targetVec[i] == 0 && vec[i] > 0)
                        scores[teacherIds[i]] = scores.GetValueOrDefault(teacherIds[i]) + vec[i] * s.Sim;
                }
            }

            return scores
                .OrderByDescending(kv => kv.Value)
                .Take(topN)
                .Select(kv => allTeachers.First(t => t.Id == kv.Key));
        }

        public static IEnumerable<TeacherEntity> RecommendTeachersByCourseHistory(AppDbContext dbContext, Guid userId, int topN = 5)
        {
            var grades = dbContext.Grades
                .Where(g => g.Students.Any(s => s.Id == userId)).Include(gradeEntity => gradeEntity.Teachers)
                .ToList();

            var teacherScores = new Dictionary<TeacherEntity, double>();

            foreach (var grade in grades)
            {
                foreach (var teacher in grade.Teachers)
                {
                    if (!teacherScores.ContainsKey(teacher))
                        teacherScores[teacher] = 0;

                    teacherScores[teacher] += grade.Grade;
                }
            }

            return teacherScores
                .OrderByDescending(kv => kv.Value)
                .Take(topN)
                .Select(kv => kv.Key);
        }
}