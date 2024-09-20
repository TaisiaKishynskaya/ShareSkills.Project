using App.Services.Concrete;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class RoleServiceTests
{
    private Mock<IRoleRepository> _roleRepositoryMock;
    private RoleService _service;

    public RoleServiceTests()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _service = new RoleService(_roleRepositoryMock.Object);
    }

    [Fact]
    public async Task AssignUserToRoleAsync_ShouldAssignRole_WhenRoleExists()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(), Name = "John Doe" };
        var roleName = "Admin";
        var roleEntity = new RoleEntity { Id = Guid.NewGuid(), Name = roleName };

        _roleRepositoryMock.Setup(repo => repo.GetUserRoleAsync(roleName))
                           .ReturnsAsync(roleEntity);

        // Act
        await _service.AssignUserToRoleAsync(user, roleName);

        // Assert
        Assert.Equal(roleEntity, user.Role);
        _roleRepositoryMock.Verify(repo => repo.GetUserRoleAsync(roleName), Times.Once);
    }

    [Fact]
    public async Task AssignUserToRoleAsync_ShouldThrowException_WhenRoleDoesNotExist()
    {
        // Arrange
        var user = new UserEntity { Id = Guid.NewGuid(), Name = "John Doe" };
        var roleName = "NonExistentRole";

        _roleRepositoryMock.Setup(repo => repo.GetUserRoleAsync(roleName))
                           .ReturnsAsync((RoleEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.AssignUserToRoleAsync(user, roleName));
        Assert.Equal("Role doesn't exists", exception.Message);
        _roleRepositoryMock.Verify(repo => repo.GetUserRoleAsync(roleName), Times.Once);
    }

    [Fact]
    public async Task GetRoleNameAsync_ShouldReturnRoleName_WhenRoleExists()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var roleName = "Admin";
        var roleEntity = new RoleEntity { Id = roleId, Name = roleName };

        _roleRepositoryMock.Setup(repo => repo.GetRoleAsync(roleId))
                           .ReturnsAsync(roleEntity);

        // Act
        var result = await _service.GetRoleNameAsync(roleId);

        // Assert
        Assert.Equal(roleName, result);
        _roleRepositoryMock.Verify(repo => repo.GetRoleAsync(roleId), Times.Once);
    }

    [Fact]
    public async Task GetRoleNameAsync_ShouldThrowException_WhenRoleDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleRepositoryMock.Setup(repo => repo.GetRoleAsync(roleId))
                           .ReturnsAsync((RoleEntity)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetRoleNameAsync(roleId));
        Assert.Equal("Role doesn't exists", exception.Message);
        _roleRepositoryMock.Verify(repo => repo.GetRoleAsync(roleId), Times.Once);
    }
}