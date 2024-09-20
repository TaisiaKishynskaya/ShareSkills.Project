using Libraries.Data;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Libraries.UnitTests.RepositoriesTests;

public class RoleRepositoryTests
{
    private readonly RoleRepository _repository;
    private readonly AppDbContext _context;
    
    public RoleRepositoryTests()
    { 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    
        _context = new AppDbContext(options);
        _repository = new RoleRepository(_context);
    }
    
    [Fact]
    public async Task GetUserRoleAsync_ShouldReturnCorrectRole()
    {
        // Arrange
        var role = new RoleEntity { Id = Guid.NewGuid(), Name = "Admin" };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserRoleAsync("Admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result?.Name);
    }
    
    [Fact]
    public async Task GetUserRoleAsync_ShouldReturnNullIfRoleNameDoesNotExist()
    {
        // Arrange
        var role = new RoleEntity { Id = Guid.NewGuid(), Name = "Manager" };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserRoleAsync("NonExistentRole");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserRoleAsync_ShouldHandleCaseSensitivity()
    {
        // Arrange
        var role = new RoleEntity { Id = Guid.NewGuid(), Name = "Developer" };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserRoleAsync("developer");

        // Assert
        Assert.Null(result); // Assuming the search is case-sensitive
    }

    [Fact]
    public async Task GetUserRoleAsync_ShouldReturnCorrectRoleWhenMultipleRolesExist()
    {
        // Arrange
        var role1 = new RoleEntity { Id = Guid.NewGuid(), Name = "Editor" };
        var role2 = new RoleEntity { Id = Guid.NewGuid(), Name = "Viewer" };
        _context.Roles.AddRange(role1, role2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserRoleAsync("Viewer");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Viewer", result?.Name);
    }

    [Fact]
    public async Task GetUserRoleAsync_ShouldReturnNullWhenDatabaseIsEmpty()
    {
        // Act
        var result = await _repository.GetUserRoleAsync("AnyRole");

        // Assert
        Assert.Null(result);
    }
    

    [Fact]
    public async Task GetRoleAsync_ShouldReturnCorrectRoleById()
    {
        // Arrange
        var role = new RoleEntity { Id = Guid.NewGuid(), Name = "User" };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRoleAsync(role.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(role.Id, result?.Id);
    }
    
    [Fact]
    public async Task GetRoleAsync_ShouldReturnNullIfRoleIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetRoleAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }
}