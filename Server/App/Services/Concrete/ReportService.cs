using App.Services.Abstract;
using Libraries.Contracts.Report;
using Libraries.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Concrete;
public class ReportService : IReportService
{
    private readonly AppDbContext _dbContext;

    public ReportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ReportDto>> GetReportForUserAsync(Guid userId)
    {
        var report = await _dbContext.Meetings
            .Where(m => m.OwnerId == userId || m.ForeignId == userId) 
            .GroupBy(m => m.SkillId) 
            .Select(group => new
            {
                SkillId = group.Key,
                TotalTime = group.Count(), 
                Themes = group.Select(m => m.Theme).Distinct().ToList(), 
                TeacherId = group.FirstOrDefault().OwnerId == userId 
                    ? group.FirstOrDefault().ForeignId 
                    : group.FirstOrDefault().OwnerId, 
            })
            .Join(
                _dbContext.Users, 
                g => g.TeacherId, 
                u => u.Id, 
                (g, u) => new { g, Teacher = u }
            )
            .Join(
                _dbContext.Skills,
                g => g.g.SkillId,
                s => s.Id,
                (g, s) => new ReportDto
                {
                    TeacherName = g.Teacher.Role.Name == "Teacher" ? g.Teacher.Name : "",
                    TeacherSurname = g.Teacher.Role.Name == "Teacher" ? g.Teacher.Surname : "",
                    SkillName = s.Skill ?? "Unknown",
                    TotalTime = g.g.TotalTime,
                    Themes = g.g.Themes
                }
            )
            .ToListAsync();

        return report;
    }
}