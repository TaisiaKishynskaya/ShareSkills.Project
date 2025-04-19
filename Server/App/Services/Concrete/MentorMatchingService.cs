using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Repositories.Abstract;
using App.Infrastructure.Exceptions.NotFoundExceptions;


namespace App.Services.Concrete;

public class MentorMatchingService
{
    private readonly RecommendationService _recommendationService;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IClassTimeService _classTimeService;
    private readonly ISkillService _skillService;
    private readonly ILevelService _levelService;
    private readonly IUnitOfWork _unitOfWork;

    public MentorMatchingService(RecommendationService recommendationService, ITeacherRepository teacherRepository, IClassTimeService classTimeService, ISkillService skillService, ILevelService levelService, IUnitOfWork unitOfWork)
    {
        _recommendationService = recommendationService;
        _teacherRepository = teacherRepository;
        _classTimeService = classTimeService;
        _skillService = skillService;
        _levelService = levelService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TeacherDto>> GetRecommendedMentors(Guid userId, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.TeacherRepository
           .GetByIdAsync(userId, cancellationToken)
            ?? throw new TeacherNotFoundException(userId);

        var userNumericId = (uint)userId.GetHashCode();
        var allTeachers = await _teacherRepository.GetAllAsync();
        var top = _recommendationService.RecommendTopTeachers(userNumericId, allTeachers.ToList());

        var levelName = await _levelService.GetLevelNameAsync(teacher.LevelId);
        var classTimeName = await _classTimeService.GetClassTimeNameAsync(teacher.ClassTimeId);
        var skillName = await _skillService.GetSkillNameAsync(teacher.SkillId);

        // Map TeacherEntity to TeacherDto
        return top.Select(t => new TeacherDto
        {
            // Assuming TeacherDto has properties similar to TeacherEntity
            Id = t.Teacher.Id,
            UserId = t.Teacher.UserId,
            Rating = t.Teacher.Rating,
            ClassTime = classTimeName,
            Level = levelName,
            Skill = skillName
        }).ToList();
    }
}
