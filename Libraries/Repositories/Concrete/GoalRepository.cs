using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class GoalRepository(AppDbContext context) : IGoalRepository
{
    public Task<GoalEntity?> GetTeacherGoalAsync(string teacherGoal) => context.Goals.FirstOrDefaultAsync(x => x.Name == teacherGoal);
    public Task<GoalEntity?> GetGoalAsync(Guid id) => context.Goals.FirstOrDefaultAsync(x => x.Id == id);
}