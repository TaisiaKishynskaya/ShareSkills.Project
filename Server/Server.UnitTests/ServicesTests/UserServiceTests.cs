using App.Infrastructure.Exceptions;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Contracts.User;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Libraries.Repositories.Abstract;
using Moq;

namespace Server.UnitTests.ServicesTests;

public class UserServiceTests
{
    // Arrange: Настраиваем необходимые зависимости и моки
    private readonly UserService _service;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    //private readonly Mock<IMapper> _mockMapper;
    //private readonly Mock<ILogger<UserService>> _mockLogger;
    
    //private readonly Guid _testId;

    public UserServiceTests()
    {
        // Arrange: 
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoleService = new Mock<IRoleService>();
        _mockUserRepository = new Mock<IUserRepository>();
        //_mockMapper = new(); _mockLogger = new();

        _service = new UserService(_mockUnitOfWork.Object, _mockRoleService.Object);
        
        //_testId = Guid.NewGuid();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers_WhenCalled()
    {
        // Arrange: Настраиваем необходимые зависимости и моки
        // Создаем тестовые данные
        var testUsers = new List<UserEntity> 
        {
            new() { Id = Guid.NewGuid(), Name = "John", Surname = "Doe", Email = "john.doe@example.com", Password = "hashed_password", RoleId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Jane", Surname = "Smith", Email = "jane.smith@example.com", Password = "hashed_password", RoleId = Guid.NewGuid() }
        };

        // Настраиваем мок UserRepository для возврата тестовых данных
        _mockUserRepository.Setup(repo => repo
                .GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(testUsers);

        // Настраиваем UnitOfWork для использования мока UserRepository
        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);
        
        // Act: Вызываем тестируемый метод
        var result = await _service.GetAllAsync();

        // Assert: Проверяем, что результат соответствует ожидаемому
        Assert.NotNull(result); // Результат не должен быть null
        Assert.Equal(2, result.Count()); // Должны вернуться 2 пользователя
        
        var firstUser = testUsers.First();
        var firstResultUser = result.First();
        
        Assert.Equal(firstUser.Id, firstResultUser.Id); // Проверяем, что Id совпадает
        Assert.Equal(firstUser.Name, firstResultUser.Name); // Имя должно совпадать
        Assert.Equal(firstUser.Surname, firstResultUser.Surname); // Фамилия должна совпадать
        //Assert.Equal("John", result.First().Name); // Имя первого пользователя должно быть "John"
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
    {
        // Arrange
        // Настраиваем мок UserRepository для возврата null
        _mockUserRepository.Setup(repo => repo
                .GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync((List<UserEntity>)null);

        // Настраиваем UnitOfWork для использования мока UserRepository
        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);

        //Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAllAsync());

        //Assert
        _mockUserRepository.Verify(repo => repo.GetAllAsync(CancellationToken.None), Times.Once);
    }
    
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var testEmail = "john.doe@example.com";
        var testRoleName = "Student";
        var testUser = new UserEntity()
        {
            Id = Guid.NewGuid(), Name = "John", Surname = "Doe", Email = testEmail, Password = "hashed_password", RoleId = Guid.NewGuid()
        };

        // Настраиваем мок UserRepository для возврата тестового пользователя
        _mockUserRepository.Setup(repo => repo
                .GetByEmailAsync(testEmail, It.IsAny<CancellationToken>())).ReturnsAsync(testUser);

        // Настраиваем мок RoleService для возврата имени роли
        _mockRoleService.Setup(service => service
            .GetRoleNameAsync(testUser.RoleId)).ReturnsAsync(testRoleName);

        _mockUnitOfWork.Setup(uow => uow.UserRepository)
            .Returns(_mockUserRepository.Object);

        // Act
        var result = await _service.GetByEmailAsync(testEmail);

        // Assert: Проверяем, что результат соответствует ожидаемому
        Assert.NotNull(result); // Результат не должен быть null
        Assert.Equal(testUser.Id, result.Id); // Id пользователя должен совпадать
        Assert.Equal(testUser.Name, result.Name); // Имя пользователя должно совпадать
        Assert.Equal(testUser.Surname, result.Surname); // Фамилия пользователя должна совпадать
        Assert.Equal(testUser.Email, result.Email); // Email должен совпадать
        Assert.Equal(testUser.Password, result.PasswordHash); // Хэш пароля должен совпадать
        Assert.Equal(testRoleName, result.Role); // Имя роли должно совпадать
    }
    
    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var testEmail = "nonexistent@example.com";

        // Настраиваем мок UserRepository для возврата null
        _mockUserRepository.Setup(repo => repo
                .GetByEmailAsync(testEmail, It.IsAny<CancellationToken>())).ReturnsAsync((UserEntity?)null);

        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);

        // Act
        var result = await _service.GetByEmailAsync(testEmail);

        // Assert: Проверяем, что результат null
        Assert.Null(result); // Ожидаем, что результат будет null
    }
    
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        // Создаем тестовые данные
        var testId = Guid.NewGuid();
        var testUser = new UserEntity()
        {
            Id = testId, Name = "John", Surname = "Doe", Email = "john.doe@example.com", Password = "hashed_password", RoleId = Guid.NewGuid()
        };

        // Настраиваем мок UserRepository для возврата тестового пользователя
        _mockUserRepository.Setup(repo => repo
                .GetByIdAsync(testId, It.IsAny<CancellationToken>())).ReturnsAsync(testUser);

        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);


        // Act
        var result = await _service.GetByIdAsync(testId);

        // Assert
        Assert.NotNull(result); // Результат не должен быть null
        Assert.Equal(testUser.Id, result.Id); // Id пользователя должен совпадать
        Assert.Equal(testUser.Name, result.Name); // Имя пользователя должно совпадать
        Assert.Equal(testUser.Surname, result.Surname); // Фамилия пользователя должна совпадать
        Assert.Equal(testUser.Email, result.Email); // Email должен совпадать
        Assert.Equal(testUser.Password, result.PasswordHash); // Хэш пароля должен совпадать
        Assert.Equal(testUser.RoleId.ToString(), result.Role); // RoleId должен быть конвертирован в строку и совпадать
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var testId = Guid.NewGuid();

        // Настраиваем мок UserRepository для возврата null
        _mockUserRepository.Setup(repo => repo
                .GetByIdAsync(testId, It.IsAny<CancellationToken>())).ReturnsAsync((UserEntity?)null);

        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);


        // Act & Assert: Проверяем, что вызов метода выбрасывает исключение UserNotFoundException
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => _service.GetByIdAsync(testId));

        // Проверяем, что исключение содержит правильное сообщение
        Assert.Equal($"The user with the identifier {testId.ToString()} was not found.", exception.Message); // Ожидаем, что идентификатор пользователя будет передан в исключении
    }


    [Fact]
    public async Task CreateAsync_ShouldReturnUserId_WhenUserIsSuccessfullyCreated()
    {
        // Arrange
        var testUserModel = new UserModel
        {
            Name = "John", Surname = "Doe", Email = "john.doe@example.com", PasswordHash = "hashed_password", Role = "Admin"
        };

        var createdUserId = Guid.NewGuid();

        // Настраиваем мок UserRepository для возврата тестового пользователя
        _mockUserRepository.Setup(repo => repo.Insert(It.IsAny<UserEntity>()))
            .Callback<UserEntity>(user => user.Id = createdUserId); // Устанавливаем Id пользователю

        _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(_mockUserRepository.Object);

        // Настраиваем мок RoleService для назначения роли
        _mockRoleService.Setup(service => service
                .AssignUserToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask); // Симулируем успешное выполнение

        _mockUnitOfWork.Setup(uow => uow
                .SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);


        // Act:
        var result = await _service.CreateAsync(testUserModel);

        // Assert: 
        Assert.Equal(createdUserId, result); // Ожидаем, что возвращаемый Id будет равен созданному Id
        _mockUserRepository.Verify(repo => repo.Insert(It.IsAny<UserEntity>()), Times.Once); // Проверяем, что Insert был вызван один раз
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Проверяем, что SaveChangesAsync был вызван один раз
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenAssignUserToRoleFails()
    {
        // Arrange:
        var testUserModel = new UserModel
        {
            Name = "John", Surname = "Doe", Email = "john.doe@example.com", PasswordHash = "hashed_password", Role = "Admin"
        };

        // Настраиваем мок RoleService для возврата исключения при назначении роли
        _mockRoleService.Setup(service => service
                .AssignUserToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Failed to assign role"));


        // Act & Assert: Проверяем, что вызов метода выбрасывает исключение
        var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(testUserModel));

        // Проверяем, что исключение содержит правильное сообщение
        Assert.Equal("Failed to assign role", exception.Message);
    
        // Verify: Проверяем, что UserRepository.Insert и SaveChangesAsync не были вызваны
        _mockUserRepository.Verify(repo => repo.Insert(It.IsAny<UserEntity>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange: Настраиваем зависимости и моки
        var testUserId = Guid.NewGuid();
        var existingUser = new UserEntity
        {
            Id = testUserId, Name = "OldName", Surname = "OldSurname", Email = "old.email@example.com", Password = "old_password_hash"
        };

        var userForUpdateDto = new UserForUpdateDto
        {
            Name = "NewName", Surname = "NewSurname", Email = "new.email@example.com", Password = "new_password"
        };

        // Настраиваем мок UserRepository для возврата существующего пользователя
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockUnitOfWork.Setup(uow => uow.UserRepository)
            .Returns(_mockUserRepository.Object);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);


        // Act: Вызываем тестируемый метод
        await _service.UpdateAsync(testUserId, userForUpdateDto);

        // Assert: Проверяем, что данные пользователя были обновлены
        Assert.Equal(userForUpdateDto.Name, existingUser.Name);
        Assert.Equal(userForUpdateDto.Surname, existingUser.Surname);
        Assert.Equal(userForUpdateDto.Email, existingUser.Email);

        // Проверяем, что метод SaveChangesAsync был вызван один раз
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange: Настраиваем зависимости и моки
        var testUserId = Guid.NewGuid();
        var userForUpdateDto = new UserForUpdateDto
        {
            Name = "NewName", Surname = "NewSurname", Email = "new.email@example.com", Password = "new_password"
        };

        // Настраиваем мок UserRepository для возврата null, когда пользователь не найден
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        _mockUnitOfWork.Setup(uow => uow.UserRepository)
            .Returns(_mockUserRepository.Object);


        // Act & Assert: Проверяем, что вызов метода выбрасывает исключение UserNotFoundException
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => _service.UpdateAsync(testUserId, userForUpdateDto));

        // Проверяем, что исключение содержит правильное сообщение
        Assert.Contains(testUserId.ToString(), exception.Message);

        // Verify: Проверяем, что метод SaveChangesAsync не был вызван
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var user = new UserEntity
        {
            Id = testId,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Password = "hashed_password"
        };

        // Настраиваем мок UserRepository для возврата пользователя
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockUnitOfWork.Setup(uow => uow.UserRepository)
            .Returns(_mockUserRepository.Object);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var userService = new UserService(_mockUnitOfWork.Object, new Mock<IRoleService>().Object);

        // Act
        await userService.DeleteAsync(testId);

        // Assert
        _mockUserRepository.Verify(repo => repo.Remove(user), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var testId = Guid.NewGuid();

        // Настраиваем мок UserRepository для возврата null
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(testId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        _mockUnitOfWork.Setup(uow => uow.UserRepository)
            .Returns(_mockUserRepository.Object);

        var userService = new UserService(_mockUnitOfWork.Object, new Mock<IRoleService>().Object);

        // Act & Assert: Проверяем, что вызов метода выбрасывает исключение UserNotFoundException
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => userService.DeleteAsync(testId));

        // Assert: Проверяем, что сообщение исключения содержит идентификатор пользователя
        Assert.Contains(testId.ToString(), exception.Message);

        // Verify: Убедитесь, что методы Remove и SaveChangesAsync не были вызваны
        _mockUserRepository.Verify(repo => repo.Remove(It.IsAny<UserEntity>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}