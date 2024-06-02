using App.Infrastructure.Exceptions;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class TeacherService(IUnitOfWork unitOfWork,
                            ITimeOfDayService timeOfDayService,
                            IGoalService goalService,
                            ISkillService skillService) : ITeacherService
{
    //private UserDto _userDto;
    
    public async Task<TeacherDto> CreateAsync(TeacherForCreationDto teacherForCreationDto, CancellationToken cancellationToken = default)
    {
        var teacher = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            Rating = teacherForCreationDto.Rating,
            TimeOfDay = teacherForCreationDto.TimeOfDay,
            Goal = teacherForCreationDto.Goal,
            Skill = teacherForCreationDto.Skill
        };

        unitOfWork.TeacherRepository.Insert(teacher);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var timeOfDayName = await timeOfDayService.GetTimeOfDayNameAsync(teacher.TimeOfDayId);
        var goal = await goalService.GetGoalNameAsync(teacher.GoalId);

        return new TeacherDto
        {
            Id = teacher.Id,
            Rating = teacher.Rating,
            TimeOfDay = timeOfDayName,
            Goal = goal,
            Skill = teacher.Skill
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
           ?? throw new TeacherNotFoundException(id);

        unitOfWork.TeacherRepository.Remove(teacher);

        //await userService.DeleteAsync(_userDto.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await unitOfWork.TeacherRepository
            .GetAllAsync(cancellationToken);

        var teachersDtos = new List<TeacherDto>();
        
        foreach (var teacher in teachers)
        {
            teachersDtos.Add(new TeacherDto
            {
                Id = teacher.Id,
                Rating = teacher.Rating,
                TimeOfDay = teacher.TimeOfDayId.ToString(),
                Goal = teacher.GoalId.ToString(),
                Skill = teacher.Skill
            });
        }

        return teachersDtos;
    }

    public async Task<TeacherDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);
        
        var timeOfDayName = await timeOfDayService.GetTimeOfDayNameAsync(teacher.TimeOfDayId);
        var goal = await goalService.GetGoalNameAsync(teacher.GoalId);
        var skill = await skillService.GetByIdAsync(teacher.SkillId);

        return new TeacherDto
        {
            Id = teacher.Id,
            Rating = teacher.Rating,
            TimeOfDay = timeOfDayName,
            Goal = goal,
            Skill = skill
        };
    }

    public async Task RecountTotalGradeAsync(Guid id, int newScore, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);

        teacher.Rating = newScore;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}