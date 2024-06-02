using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class TimeOfDayService(ITimeOfDayRepository timeOfDayRepository) : ITimeOfDayService
{
    public async Task AssignTeacherToTimeOfDayAsync(TeacherEntity teacher, string timeOfDay)
    {
        teacher.TimeOfDay = await timeOfDayRepository.GetTeacherTimeOfDayAsync(timeOfDay) ?? throw new Exception("Time of day doesn't exists");
    }

    public async Task<string> GetTimeOfDayNameAsync(Guid id)
    {
        var timeOfDay = await timeOfDayRepository.GetTimeOfDayAsync(id) ?? throw new Exception("Time of day doesn't exists");
        return timeOfDay.Name;
    }
}