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
                StudentId = group.FirstOrDefault().OwnerId == userId 
                    ? group.FirstOrDefault().OwnerId 
                    : group.FirstOrDefault().ForeignId
            })
            .Join(
                _dbContext.Users, 
                g => g.TeacherId, 
                u => u.Id, 
                (g, u) => new { g, Teacher = u }
            )
            .Join(
                _dbContext.Users,
                g => g.g.StudentId,
                u => u.Id,
                (g, u) => new { g, Student = u }
            )
            .Join(
                _dbContext.Skills,
                g => g.g.g.SkillId,
                s => s.Id,
                (g, s) => new ReportDto
                {
                    TeacherName = g.g.Teacher.Role.Name == "Teacher" ? g.g.Teacher.Name : "",
                    TeacherSurname = g.g.Teacher.Role.Name == "Teacher" ? g.g.Teacher.Surname : "",
                    StudentName = g.Student.Name, 
                    StudentSurname = g.Student.Surname, 
                    SkillName = s.Skill ?? "Unknown",
                    TotalTime = g.g.g.TotalTime,
                    Themes = g.g.g.Themes
                }
            )
            .ToListAsync();

        return report;
    }
}