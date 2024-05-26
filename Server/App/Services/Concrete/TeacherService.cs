using App.Infrastructure.Exceptions;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class TeacherService(IUnitOfWork unitOfWork) : ITeacherService
{
    //private UserDto _userDto;
    
    public async Task<TeacherDto> CreateAsync(TeacherForCreationDto teacherForCreationDto, CancellationToken cancellationToken = default)
    {
        var teacher = new TeacherEntity
        {
            Id = Guid.NewGuid(),
            Rating = teacherForCreationDto.Rating,
        };

        unitOfWork.TeacherRepository.Insert(teacher);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TeacherDto
        {
            Id = teacher.Id,
            Rating = teacher.Rating
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
            });
        }

        return teachersDtos;
    }

    public async Task<TeacherDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);

        return new TeacherDto
        {
            Id = teacher.Id,
            Rating = teacher.Rating
        };
    }

    public async Task UpdateAsync(Guid id, TeacherForUpdateDto teacherForUpdateDto, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);

        teacher.Rating = teacherForUpdateDto.Rating;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}