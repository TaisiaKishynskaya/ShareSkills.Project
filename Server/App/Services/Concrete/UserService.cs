using App.Infrastructure.Exceptions.NotFoundExceptions;
using App.Services.Abstract;
using Libraries.Contracts.User;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Entities.Concrete;
using Microsoft.AspNetCore.Identity;

namespace App.Services.Concrete;

public class UserService(IUnitOfWork unitOfWork, IRoleService roleService, ICacheService cacheService) : IUserService
{

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user:{email}";
        
        var cachedUser = await cacheService.GetCacheValueAsync<UserDto>(cacheKey);
        if (cachedUser != null)
        {
            return cachedUser;
        }
        
        var user = await unitOfWork.UserRepository
            .GetByEmailAsync(email, cancellationToken);
        
        if (user is not null)
        {
            //TODO: need to be replaced by EF mapping mechanism for map it automatically
            var roleName = await roleService.GetRoleNameAsync(user.RoleId);
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                PasswordHash = user.Password,
                Email = user.Email,
                Role = roleName
            };
            
            await cacheService.SetCacheValueAsync(cacheKey, userDto);
            return userDto;
        }

        return null;
    }

    public async Task<Guid> CreateAsync(UserModel userForCreationDto, CancellationToken cancellationToken = default)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = userForCreationDto.Name,
            Surname = userForCreationDto.Surname,
            Email = userForCreationDto.Email,
            Password = userForCreationDto.PasswordHash
        };
        await roleService.AssignUserToRoleAsync(user, userForCreationDto.Role);
        unitOfWork.UserRepository.Insert(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository
           .GetByIdAsync(id, cancellationToken)
           ?? throw new UserNotFoundException(id);

        unitOfWork.UserRepository.Remove(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var cacheKeyById = $"user:{id}";
        var cacheKeyAllUsers = "all_users";

        await cacheService.DeleteCacheValueAsync(cacheKeyById);
        await cacheService.DeleteCacheValueAsync(cacheKeyAllUsers);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "all_users";
    
        var cachedUsers = await cacheService.GetCacheValueAsync<List<UserDto>>(cacheKey);
        if (cachedUsers != null)
        {
            return cachedUsers;
        }

        var users = await unitOfWork.UserRepository.GetAllAsync(cancellationToken);

        var userDtos = users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PasswordHash = user.Password,
            Role = user.RoleId.ToString()
        }).ToList();
        
        await cacheService.SetCacheValueAsync(cacheKey, userDtos);
        return userDtos;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user:{id}";
        
        var cachedUser = await cacheService.GetCacheValueAsync<UserDto>(cacheKey);
        if (cachedUser != null)
        {
            return cachedUser;
        }
        
        var user = await unitOfWork.UserRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new UserNotFoundException(id);

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PasswordHash = user.Password,
            Role = user.RoleId.ToString()
        };
        
        await cacheService.SetCacheValueAsync(cacheKey, userDto);
        return userDto;
    }

    public async Task UpdateAsync(Guid id, UserForUpdateDto userForUpdateDto, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.UserRepository
           .GetByIdAsync(id, cancellationToken)
            ?? throw new UserNotFoundException(id);

        user.Name = userForUpdateDto.Name;
        user.Surname = userForUpdateDto.Surname;
        user.Email = userForUpdateDto.Email;
        
        
        var passwordHasher = new PasswordHasher<UserEntity>();
        var passwordHash = passwordHasher.HashPassword(user, userForUpdateDto.Password);
        Console.Write("hash: "+passwordHash);

        user.Password = passwordHash;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await cacheService.DeleteCacheValueAsync($"user:{id}");
        await cacheService.DeleteCacheValueAsync($"user:{user.Email}");
    }
}