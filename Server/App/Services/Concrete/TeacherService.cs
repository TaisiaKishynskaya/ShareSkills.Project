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
            Name = teacherForCreationDto.Name,
            Surname = teacherForCreationDto.Surname,
            Email = teacherForCreationDto.Email,
            Password = teacherForCreationDto.Password
        };

        unitOfWork.TeacherRepository.Insert(teacher);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TeacherDto
        {
            Id = teacher.Id,
            Rating = teacher.Rating,
            Name = teacher.Name,
            Surname = teacher.Surname,
            Email = teacher.Email,
            Password = teacher.Password
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
                Name = teacher.Name,
                Surname = teacher.Surname,
                Email = teacher.Email,
                Password = teacher.Password
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
            Rating = teacher.Rating,
            Name = teacher.Name,
            Surname = teacher.Surname,
            Email = teacher.Email,
            Password = teacher.Password
        };
    }

    public async Task UpdateAsync(Guid id, TeacherForUpdateDto teacherForUpdateDto, CancellationToken cancellationToken = default)
    {
        var teacher = await unitOfWork.TeacherRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new TeacherNotFoundException(id);

        teacher.Rating = teacherForUpdateDto.Rating;
        teacher.Name = teacherForUpdateDto.Name;
        teacher.Surname = teacherForUpdateDto.Surname;
        teacher.Email = teacherForUpdateDto.Email;
        teacher.Password = teacherForUpdateDto.Password;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}