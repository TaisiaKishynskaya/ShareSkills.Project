using Libraries.Entities.Concrete;

namespace Libraries.Repositories.Abstract;

public interface IMeetingRepository
{
    Task<IEnumerable<MeetingEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MeetingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MeetingEntity?> GetExistedAsync(Guid id, DateTime dateTime, CancellationToken cancellationToken = default);
    void Insert(MeetingEntity meeting);
    void Remove(MeetingEntity meeting);
}