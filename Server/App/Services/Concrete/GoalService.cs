using App.Services.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;

namespace App.Services.Concrete;

public class GoalService(IGoalRepository goalRepository) : IGoalService
{
    public async Task AssignTeacherToGoalAsync(TeacherEntity teacher, string goal)
    {
        teacher.Goal = await goalRepository.GetTeacherGoalAsync(goal) ?? throw new Exception("Goal doesn't exists");
    }

    public async Task<string> GetGoalNameAsync(Guid id)
    {
        var goal = await goalRepository.GetGoalAsync(id) ?? throw new Exception("Goal doesn't exists");
        return goal.Name;
    }
}