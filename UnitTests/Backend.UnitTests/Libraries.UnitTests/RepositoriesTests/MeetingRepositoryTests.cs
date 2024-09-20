using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class MeetingRepositoryTests
{
    private readonly MeetingRepository _repository;
    private readonly AppDbContext _context;
    
    public MeetingRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new MeetingRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMeetings()
    {
        // Arrange
        var meeting1 = new MeetingEntity { Id = Guid.NewGuid(), Name = "Meeting 1", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid()};
        var meeting2 = new MeetingEntity { Id = Guid.NewGuid(), Name = "Meeting 2", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };
        _context.Meetings.AddRange(meeting1, meeting2);
        await _context.SaveChangesAsync();

        // Act
        var meetings = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, meetings.Count());
        Assert.Contains(meetings, m => m.Name == "Meeting 1");
        Assert.Contains(meetings, m => m.Name == "Meeting 2");
    }

    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectMeeting()
    {
        // Arrange
        var meeting = new MeetingEntity { Id = Guid.NewGuid(), Name = "Meeting", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(meeting.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meeting.Id, result?.Id);
    }

    
    [Fact]
    public async Task GetExistedAsync_ShouldReturnMeetingIfExists()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        var meeting = new MeetingEntity { Id = Guid.NewGuid(), Name = "Meeting", DateTime = dateTime, OwnerId = ownerId, ForeignId = Guid.NewGuid() };
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetExistedAsync(ownerId, dateTime);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meeting.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetExistedAsync_ShouldReturnNullIfMeetingDoesNotExist()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;

        // Act
        var result = await _repository.GetExistedAsync(ownerId, dateTime);

        // Assert
        Assert.Null(result);
    }
    

    [Fact]
    public void Insert_ShouldAddMeeting()
    {
        // Arrange
        var meeting = new MeetingEntity { Id = Guid.NewGuid(), Name = "New Meeting", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };

        // Act
        _repository.Insert(meeting);
        _context.SaveChanges();

        // Assert
        var result = _context.Meetings.Find(meeting.Id);
        Assert.NotNull(result);
        Assert.Equal(meeting.Name, result?.Name);
    }
    
    [Fact]
    public void Insert_ShouldNotAddMeetingWithDuplicateId()
    {
        // Arrange
        var meetingId = Guid.NewGuid();
        var meeting1 = new MeetingEntity { Id = meetingId, Name = "Meeting 1", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };
        _repository.Insert(meeting1);
        _context.SaveChanges();

        var meeting2 = new MeetingEntity { Id = meetingId, Name = "Meeting 2", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            _repository.Insert(meeting2);
            _context.SaveChanges();
        });

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    
    [Fact]
    public void Remove_ShouldDeleteMeeting()
    {
        // Arrange
        var meeting = new MeetingEntity { Id = Guid.NewGuid(), Name = "Meeting to Delete", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };
        _context.Meetings.Add(meeting);
        _context.SaveChanges();

        // Act
        _repository.Remove(meeting);
        _context.SaveChanges();

        // Assert
        var result = _context.Meetings.Find(meeting.Id);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Remove_ShouldThrowDbUpdateConcurrencyExceptionIfMeetingDoesNotExist()
    {
        // Arrange
        var meeting = new MeetingEntity { Id = Guid.NewGuid(), Name = "Nonexistent Meeting", DateTime = DateTime.UtcNow, OwnerId = Guid.NewGuid(), ForeignId = Guid.NewGuid() };

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
        {
            _repository.Remove(meeting);
            await _context.SaveChangesAsync();
        });

        Assert.IsType<DbUpdateConcurrencyException>(exception);
    }
}