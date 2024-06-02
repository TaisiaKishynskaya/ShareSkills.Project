using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class TimeOfDayRepository(AppDbContext context) : ITimeOfDayRepository
{
    public Task<TimeOfDayEntity?> GetTeacherTimeOfDayAsync(string teacherTimeOfDay) => context.TimeOfDays.FirstOrDefaultAsync(x => x.Name == teacherTimeOfDay);
    public Task<TimeOfDayEntity?> GetTimeOfDayAsync(Guid id) => context.TimeOfDays.FirstOrDefaultAsync(x => x.Id == id);
}