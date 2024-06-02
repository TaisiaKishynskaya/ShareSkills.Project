using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IGoalRepository
{
    Task<GoalEntity?> GetTeacherGoalAsync(string teacherGoal);

    Task<GoalEntity?> GetGoalAsync(Guid id);
}