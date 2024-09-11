using App.Infrastructure.Exceptions.AlreadyExistsExceptions;
using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Concrete;
using Libraries.Contracts.Meeting;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class MeetingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MeetingService _service;

    public MeetingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new MeetingService(_unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task TryToCreateAsync_ShouldThrowMeetingAlreadyExistsException_WhenMeetingExists()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var dateAndTime = DateTime.UtcNow;
        var meetingDto = new MeetingForCreatingDto
        {
            OwnerId = ownerId,
            ForeignId = Guid.NewGuid(),
            DateAndTime = dateAndTime,
            Name = "Test Meeting"
        };

        // Настроить, чтобы метод GetExistedAsync возвращал существующее собрание
        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetExistedAsync(ownerId, dateAndTime, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new MeetingEntity
                       {
                           Id = default,
                           Name = null,
                           DateTime = default,
                           OwnerId = default,
                           ForeignId = default
                       });

        // Act & Assert
        await Assert.ThrowsAsync<MeetingAlreadyExistsException>(() => _service.TryToCreateAsync(meetingDto));
    }

    [Fact]
    public async Task TryToCreateAsync_ShouldCreateMeeting_WhenMeetingDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var foreignId = Guid.NewGuid();
        var dateAndTime = DateTime.UtcNow;
        var meetingDto = new MeetingForCreatingDto
        {
            OwnerId = ownerId,
            ForeignId = foreignId,
            DateAndTime = dateAndTime,
            Name = "Test Meeting"
        };

        // Настроить, чтобы метод GetExistedAsync возвращал null, что означает, что встречи не существует
        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetExistedAsync(ownerId, dateAndTime, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((MeetingEntity)null);

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.Insert(It.IsAny<MeetingEntity>()));

        // Act
        var result = await _service.TryToCreateAsync(meetingDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meetingDto.OwnerId, result.OwnerId);
        Assert.Equal(meetingDto.ForeignId, result.ForeignId);
        Assert.Equal(meetingDto.DateAndTime, result.DateTime);
        Assert.Equal(meetingDto.Name, result.Name);

        _unitOfWorkMock.Verify(uow => uow.MeetingRepository.Insert(It.IsAny<MeetingEntity>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveMeeting_WhenMeetingExists()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var meeting = new MeetingEntity { Id = meetingId, Name = "Math Lesson", DateTime = default, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meeting);

        // Act
        await _service.DeleteAsync(meetingId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.MeetingRepository.Remove(meeting), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowMeetingNotFoundException_WhenMeetingDoesNotExist()
    {
        // Arrange
        var meetingId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MeetingEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MeetingNotFoundException>(() => _service.DeleteAsync(meetingId));
        Assert.Equal($"The meeting with the identifier {meetingId} was not found.", exception.Message);

        _unitOfWorkMock.Verify(uow => uow.MeetingRepository.Remove(It.IsAny<MeetingEntity>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMeetings_WhenMeetingsExist()
    {
        // Arrange
        var meetings = new List<MeetingEntity>
        {
            new MeetingEntity
            {
                Id = Guid.NewGuid(),
                OwnerId = Guid.NewGuid(),
                ForeignId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                Name = "Meeting 1",
                Description = "Description 1"
            },
            new MeetingEntity
            {
                Id = Guid.NewGuid(),
                OwnerId = Guid.NewGuid(),
                ForeignId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow.AddDays(1),
                Name = "Meeting 2",
                Description = "Description 2"
            }
        };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(meetings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meetings.Count, result.Count());

        foreach (var meeting in meetings)
        {
            var dto = result.SingleOrDefault(m => m.Id == meeting.Id);
            Assert.NotNull(dto);
            Assert.Equal(meeting.OwnerId, dto.OwnerId);
            Assert.Equal(meeting.ForeignId, dto.ForeignId);
            Assert.Equal(meeting.DateTime, dto.DateTime);
            Assert.Equal(meeting.Name, dto.Name);
            Assert.Equal(meeting.Description, dto.Description);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoMeetingsExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<MeetingEntity>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnMeetingDto_WhenMeetingExists()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var meeting = new MeetingEntity
        {
            Id = meetingId,
            OwnerId = Guid.NewGuid(),
            ForeignId = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            Name = "Meeting Name",
            Description = "Meeting Description"
        };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meeting);

        // Act
        var result = await _service.GetByIdAsync(meetingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meeting.Id, result.Id);
        Assert.Equal(meeting.OwnerId, result.OwnerId);
        Assert.Equal(meeting.ForeignId, result.ForeignId);
        Assert.Equal(meeting.DateTime, result.DateTime);
        Assert.Equal(meeting.Name, result.Name);
        Assert.Equal(meeting.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowMeetingNotFoundException_WhenMeetingDoesNotExist()
    {
        // Arrange
        var meetingId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MeetingEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MeetingNotFoundException>(() => _service.GetByIdAsync(meetingId));
        Assert.Equal($"The meeting with the identifier {meetingId} was not found.", exception.Message);
    }
    
    
    [Fact]
    public async Task GetExistedMeeting_ShouldReturnMeetingDto_WhenMeetingExists()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        var meeting = new MeetingEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = entityId,
            ForeignId = Guid.NewGuid(),
            DateTime = dateTime,
            Name = "Meeting Name",
            Description = "Meeting Description"
        };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetExistedAsync(entityId, dateTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meeting);

        // Act
        var result = await _service.GetExistedMeeting(entityId, dateTime);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meeting.Id, result.Id);
        Assert.Equal(meeting.OwnerId, result.OwnerId);
        Assert.Equal(meeting.ForeignId, result.ForeignId);
        Assert.Equal(meeting.DateTime, result.DateTime);
        Assert.Equal(meeting.Name, result.Name);
        Assert.Equal(meeting.Description, result.Description);
    }

    [Fact]
    public async Task GetExistedMeeting_ShouldThrowMeetingNotFoundException_WhenMeetingDoesNotExist()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetExistedAsync(entityId, dateTime, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MeetingEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MeetingNotFoundException>(() => _service.GetExistedMeeting(entityId, dateTime));
        Assert.Equal($"The meeting with the identifier {entityId} was not found.", exception.Message);
    }

    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateMeetingDateTime_WhenMeetingExists()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var newDateTime = DateTime.UtcNow.AddDays(1);
        var meeting = new MeetingEntity
        {
            Id = meetingId,
            DateTime = DateTime.UtcNow,
            Name = null,
            OwnerId = default,
            ForeignId = default
        };

        var updateDto = new MeetingForUpdateDto
        {
            DateAndTime = newDateTime
        };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(meeting);

        // Act
        await _service.UpdateAsync(meetingId, updateDto);

        // Assert
        Assert.Equal(newDateTime, meeting.DateTime);
        _unitOfWorkMock.Verify(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowMeetingNotFoundException_WhenMeetingDoesNotExist()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var updateDto = new MeetingForUpdateDto
        {
            DateAndTime = DateTime.UtcNow
        };

        _unitOfWorkMock.Setup(uow => uow.MeetingRepository.GetByIdAsync(meetingId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((MeetingEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MeetingNotFoundException>(() => _service.UpdateAsync(meetingId, updateDto));
        Assert.Equal($"The meeting with the identifier {meetingId} was not found.", exception.Message);
    }
}