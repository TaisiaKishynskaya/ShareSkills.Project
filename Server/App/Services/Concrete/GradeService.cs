using App.Infrastructure.Exceptions;
using App.Services.Abstract;
using Libraries.Contracts.Grade;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class GradeService(IUnitOfWork unitOfWork, ITeacherService teacherService) : IGradeService
{
    public async Task<GradeDto> CreateAsync(GradeForCreatingDto gradeForCreatingDto, CancellationToken cancellationToken = default)
    {
        var grade = new GradeEntity
        {
            Id = Guid.NewGuid(),
            Grade = gradeForCreatingDto.Grade,
        };

        unitOfWork.GradeRepository.Insert(grade);
        var teacherScores = await unitOfWork.TeacherRepository.GetScoresByTeacherIdAsync(gradeForCreatingDto.TeacherId);
        teacherScores.ToList().Add(grade.Grade);
        var result = teacherScores.Sum()/teacherScores.ToArray().Length;
        await teacherService.RecountTotalGradeAsync(gradeForCreatingDto.TeacherId, result, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GradeDto
        {
            Id = grade.Id,
            Grade = result
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var grade = await unitOfWork.GradeRepository
                        .GetByIdAsync(id, cancellationToken)
                    ?? throw new GradeNotFoundException(id);

        unitOfWork.GradeRepository.Remove(grade);

        //await userService.DeleteAsync(_userDto.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<GradeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var grades = await unitOfWork.GradeRepository
            .GetAllAsync(cancellationToken);

        var gradesDtos = new List<GradeDto>();

        foreach (var grade in grades)
        {
            gradesDtos.Add(new GradeDto
            {
                Id = grade.Id,
                Grade = grade.Grade,
            });
        }

        return gradesDtos;
    }

    public async Task<GradeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var grade = await unitOfWork.GradeRepository
                        .GetByIdAsync(id, cancellationToken)
                    ?? throw new GradeNotFoundException(id);

        return new GradeDto
        {
            Id = grade.Id,
            Grade = grade.Grade
        };
    }

    public async Task UpdateAsync(Guid id, GradeForUpdateDto gradeForUpdateDto, CancellationToken cancellationToken = default)
    {
        var grade = await unitOfWork.GradeRepository
                        .GetByIdAsync(id, cancellationToken)
                    ?? throw new GradeNotFoundException(id);

        grade.Grade = gradeForUpdateDto.Grade;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}