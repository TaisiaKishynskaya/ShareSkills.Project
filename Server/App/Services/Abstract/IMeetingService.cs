using Libraries.Contracts.Meeting;

namespace App.Services.Abstract;

public interface IMeetingService
{
    Task<IEnumerable<MeetingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MeetingDto>> GetAllByWeekAsync(DateTime startOfWeek, DateTime endOfWeek, CancellationToken cancellationToken = default);
    Task<MeetingDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MeetingDto> GetExistedMeeting(Guid entityId, DateTime dateTime, CancellationToken cancellationToken = default);
    Task<MeetingDto> TryToCreateAsync(MeetingForCreatingDto studentForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, MeetingForUpdateDto studentForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}