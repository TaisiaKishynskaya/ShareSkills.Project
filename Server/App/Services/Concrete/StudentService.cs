using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using Libraries.Contracts.Student;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class StudentService (IUnitOfWork unitOfWork) : IStudentService
{
    public async Task<StudentDto> CreateAsync(StudentForCreationDto studentForCreationDto, CancellationToken cancellationToken = default)
    {
        var student = new StudentEntity
        {
            Id = Guid.NewGuid(),
            Purpose = studentForCreationDto.Purpose,
            UserId = default,
        };

        unitOfWork.StudentRepository.Insert(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentDto
        {
            Id = student.Id,
            Purpose = student.Purpose
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.StudentRepository
            .GetByIdAsync(id, cancellationToken)
            ?? throw new StudentNotFoundException(id);

        unitOfWork.StudentRepository.Remove(student);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var students = await unitOfWork.StudentRepository
            .GetAllAsync(cancellationToken);

        var studentDtos = new List<StudentDto>();

        foreach (var student in students)
        {
            studentDtos.Add(new StudentDto
            {
                Id = student.Id,
                Purpose = student.Purpose
            });
        }

        return studentDtos;
    }

    public async Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.StudentRepository
            .GetByIdAsync(id, cancellationToken)
            ?? throw new StudentNotFoundException(id);

        return new StudentDto
        {
            Id = student.Id,
            Purpose = student.Purpose
        };
    }

    public async Task UpdateAsync(Guid id, StudentForUpdateDto studentForUpdateDto, CancellationToken cancellationToken = default)
    {
        var student = await unitOfWork.StudentRepository
            .GetByIdAsync(id, cancellationToken)
            ?? throw new StudentNotFoundException(id);

        student.Purpose = studentForUpdateDto.Purpose;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}