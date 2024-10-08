using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class TeacherService(IUnitOfWork unitOfWork,
                            ILevelService levelService,
                            IClassTimeService classTimeService,
                            ISkillService skillService) : ITeacherService
{
    //private UserDto _userDto;
    
     public async Task<TeacherDto> CreateAsync(TeacherForCreationDto teacherForCreationDto, CancellationToken cancellationToken = default)
    {
        var skillEntity = await unitOfWork.SkillRepository.GetTeacherSkillAsync(teacherForCreationDto.Skill);
        if (skillEntity == null)
        {
            throw new InvalidOperationException($"Skill '{teacherForCreationDto.Skill}' not found.");
        }

        var levelEntity = await unitOfWork.LevelRepository.GetTeacherLevelAsync(teacherForCreationDto.Level);
        if (levelEntity == null)
        {
            throw new InvalidOperationException($"Level '{teacherForCreationDto.Level}' not found.");
        }

        var classTimeEntity = await unitOfWork.ClassTimeRepository.GetTeacherClassTimeAsync(teacherForCreationDto.ClassTime);
        if (classTimeEntity == null)
        {
            throw new InvalidOperationException($"ClassTime '{teacherForCreationDto.ClassTime}' not found.");
        }

        var teacher = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            Rating = teacherForCreationDto.Rating,
            LevelId = levelEntity.Id,
            ClassTimeId = classTimeEntity.Id,
            SkillId = skillEntity.Id,
            UserId = teacherForCreationDto.UserId
        };

        unitOfWork.TeacherRepository.Insert(teacher);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var levelName = await levelService.GetLevelNameAsync(teacher.LevelId);
        var classTimeName = await classTimeService.GetClassTimeNameAsync(teacher.ClassTimeId);
        var skillName = await skillService.GetSkillNameAsync(teacher.SkillId);

        return new TeacherDto
        {
            Id = teacher.Id,
            UserId = teacher.UserId,
            Rating = teacher.Rating,
            ClassTime = classTimeName,
            Level = levelName,
            Skill = skillName
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
            var levelName = await levelService.GetLevelNameAsync(teacher.LevelId);
            var classTimeName = await classTimeService.GetClassTimeNameAsync(teacher.ClassTimeId);
            var skillName = await skillService.GetSkillNameAsync(teacher.SkillId);

            teachersDtos.Add(new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                Rating = teacher.Rating,
                ClassTime = classTimeName,
                Level = levelName,
                Skill = skillName
            });
        }

        return teachersDtos;
    }

    public async Task<TeacherExtendedDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);
        
        var user = await unitOfWork.UserRepository.GetByIdAsync(teacher.UserId, cancellationToken);
        
        var levelName = await levelService.GetLevelNameAsync(teacher.LevelId);
        var classTimeName = await classTimeService.GetClassTimeNameAsync(teacher.ClassTimeId);
        var skillName = await skillService.GetSkillNameAsync(teacher.SkillId);
        
        return new TeacherExtendedDto
        {
            Id = teacher.Id,
            UserId = teacher.UserId,
            Rating = teacher.Rating,
            ClassTime = classTimeName,
            Level = levelName,
            Skill = skillName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email
        };
    }

    public async Task<Guid?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByEmailAsync(email, cancellationToken);
        if (teacher != null)
        {
            return teacher.Id;
        }
        return null;
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