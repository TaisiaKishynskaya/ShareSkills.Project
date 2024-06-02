using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface ITimeOfDayRepository
{
    Task<TimeOfDayEntity?> GetTeacherTimeOfDayAsync(string teacherTimeOfDay);

    Task<TimeOfDayEntity?> GetTimeOfDayAsync(Guid id);
}