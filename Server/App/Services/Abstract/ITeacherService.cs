using Libraries.Contracts.Teacher;

namespace App.Services.Abstract;

public interface ITeacherService
{
    Task<IEnumerable<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TeacherDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeacherDto> CreateAsync(TeacherForCreationDto teacherForCreationDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task RecountTotalGradeAsync(Guid teacherId, int grade, CancellationToken cancellationToken);
}