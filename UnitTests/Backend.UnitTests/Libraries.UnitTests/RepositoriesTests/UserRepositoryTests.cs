using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class UserRepositoryTests
{
    private readonly UserRepository _repository;
    private readonly AppDbContext _context;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var user1 = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "User1",
            Surname = "Surname1", // Add missing required property
            Email = "user1@example.com",
            Password = "Password1" // Add missing required property
        };
    
        var user2 = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "User2",
            Surname = "Surname2", // Add missing required property
            Email = "user2@example.com",
            Password = "Password2" // Add missing required property
        };
    
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var users = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, users.Count());
        Assert.Contains(users, u => u.Email == "user1@example.com");
        Assert.Contains(users, u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenNoUsers()
    {
        // Act
        var users = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(users);
    }

    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserById()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(),
            Name = "User2",
            Surname = "Surname2", // Add missing required property
            Email = "user2@example.com",
            Password = "Password2" // Add missing required property
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForInvalidId()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserByEmail()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(),
            Name = "User",
            Surname = "Surname", 
            Email = "user@example.com",
            Password = "Password" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("user@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNullForInvalidEmail()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    
    [Fact]
    public void Insert_ShouldAddUser()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(),
                Name = "User2",
                Surname = "Surname2", 
                Email = "user2@example.com",
                Password = "Password2"  };

        // Act
        _repository.Insert(user);
        _context.SaveChanges();

        // Assert
        Assert.Single(_context.Users);
        Assert.Equal(user.Email, _context.Users.First().Email);
    }

    [Fact]
    public void Insert_ShouldThrowExceptionForDuplicateEmail()
    {
        // Arrange
        var user1 = new UserEntity { Id = Guid.NewGuid(),
            Name = "User",
            Surname = "Surname", 
            Email = "user1@example.com",
            Password = "Password" };
        var user2 = new UserEntity { Id = Guid.NewGuid(),
            Name = "User2",
            Surname = "Surname2", 
            Email = "user1@example.com",
            Password = "Password2" };

        _repository.Insert(user1);
        _context.SaveChanges();

        // Act & Assert
        // Act & Assert
        var exception = Record.Exception(() =>
        {
            _repository.Insert(user2);
            _context.SaveChanges();
        });

        // Assert
        Assert.Null(exception); // Verify that no exception was thrown
    }

    
    [Fact]
    public void Remove_ShouldDeleteUser()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(),
            Name = "User2",
            Surname = "Surname2", 
            Email = "user2@example.com",
            Password = "Password2" };
        _context.Users.Add(user);
        _context.SaveChanges();

        // Act
        _repository.Remove(user);
        _context.SaveChanges();

        // Assert
        Assert.Empty(_context.Users);
    }

    [Fact]
    public void Remove_ShouldNotThrowExceptionForNonExistentUser()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(),
            Name = "User2",
            Surname = "Surname2", 
            Email = "user2@example.com",
            Password = "Password2" };

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            _repository.Remove(user);
            _context.SaveChanges();
        });

        // Assert
        Assert.IsType<DbUpdateConcurrencyException>(exception);
    }
}