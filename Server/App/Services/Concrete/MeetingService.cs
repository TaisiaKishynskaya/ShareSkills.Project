using App.Infrastructure.Exceptions;
using App.Infrastructure.Exceptions.AlreadyExistsExceptions;
using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using Libraries.Contracts.Meeting;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class MeetingService(IUnitOfWork unitOfWork) : IMeetingService
{
    public async Task<MeetingDto> TryToCreateAsync(MeetingForCreatingDto meetingForCreatingDto, CancellationToken cancellationToken = default)
    {
        var isMeeting = await IsMeetingExist(meetingForCreatingDto.OwnerId, meetingForCreatingDto.DateAndTime, cancellationToken);
        if (isMeeting)
        {
            throw new MeetingAlreadyExistsException("Meeting already exists");
        }
        
        var meeting = new MeetingEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = meetingForCreatingDto.OwnerId,
            ForeignId = meetingForCreatingDto.ForeignId,
            DateTime = meetingForCreatingDto.DateAndTime,
            Name = meetingForCreatingDto.Name
        };

        unitOfWork.MeetingRepository.Insert(meeting);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MeetingDto
        {
            Id = meeting.Id,
            OwnerId = meeting.OwnerId,
            ForeignId = meeting.ForeignId,
            DateTime = meeting.DateTime,
            Description = meeting.Description,
            Name = meeting.Name
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var meeting = await unitOfWork.MeetingRepository
                          .GetByIdAsync(id, cancellationToken)
                      ?? throw new MeetingNotFoundException(id);

        unitOfWork.MeetingRepository.Remove(meeting);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MeetingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var meetings = await unitOfWork.MeetingRepository
            .GetAllAsync(cancellationToken);
        
        var meetingsDtos = meetings.Select(entity => new MeetingDto
        {
            Id = entity.Id,
            OwnerId = entity.OwnerId,
            ForeignId = entity.ForeignId,
            DateTime = entity.DateTime,
            Description = entity.Description,
            Name = entity.Name
        });
        
        return meetingsDtos;
    }

    public async Task<MeetingDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var meeting = await unitOfWork.MeetingRepository
                          .GetByIdAsync(id, cancellationToken)
                      ?? throw new MeetingNotFoundException(id);

        return new MeetingDto
        {
            Id = meeting.Id,
            OwnerId = meeting.OwnerId,
            ForeignId = meeting.ForeignId,
            DateTime = meeting.DateTime,
            Description = meeting.Description,
            Name = meeting.Name
        };
    }

    public async Task<MeetingDto> GetExistedMeeting(Guid entityId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var meeting = await unitOfWork.MeetingRepository.GetExistedAsync(entityId, dateTime, cancellationToken)
                      ?? throw new MeetingNotFoundException(entityId);

        return new MeetingDto
        {
            Id = meeting.Id,
            OwnerId = meeting.OwnerId,
            ForeignId = meeting.ForeignId,
            DateTime = meeting.DateTime,
            Description = meeting.Description,
            Name = meeting.Name
        };
    }

    private async Task <bool> IsMeetingExist(Guid entityId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        return (await unitOfWork.MeetingRepository.GetExistedAsync(entityId, dateTime, cancellationToken) == null) ? false : true; 
    }

    public async Task UpdateAsync(Guid id, MeetingForUpdateDto meetingForUpdateDto, CancellationToken cancellationToken = default)
    {
        var meeting = await unitOfWork.MeetingRepository
                          .GetByIdAsync(id, cancellationToken)
                      ?? throw new MeetingNotFoundException(id);

        meeting.DateTime = meetingForUpdateDto.DateAndTime;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}