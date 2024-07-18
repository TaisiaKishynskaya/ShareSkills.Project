using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Libraries.Repositories.Concrete;

public class MeetingRepository(AppDbContext context) : IMeetingRepository
{
    public async Task<IEnumerable<MeetingEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Meetings.ToListAsync(cancellationToken);

    public Task<MeetingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Meetings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<MeetingEntity?> GetExistedAsync(Guid id, DateTime dateTime, CancellationToken cancellationToken = default) => 
        context.Meetings.FirstOrDefaultAsync(x => (x.OwnerId == id || x.ForeignId == id) && x.DateTime == dateTime, cancellationToken: cancellationToken);

    public void Insert(MeetingEntity meeting)
        => context.Meetings.Add(meeting);

    public void Remove(MeetingEntity meeting)
        => context.Meetings.Remove(meeting);
}