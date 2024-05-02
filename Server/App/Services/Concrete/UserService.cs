// using App.Infrastructure.Exceptions;
// using App.Services.Abstract;
// using Libraries.Contracts.User;
// using Libraries.Data.UnitOfWork.Abstract;
// using Libraries.Entities.Concrete;
//
// namespace App.Services.Concrete;
//
// public class UserService(IUnitOfWork unitOfWork) : IUserService
// {
//     public async Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
//     {
//         var user = new UserEntity
//         {
//             Id = Guid.NewGuid(),
//             Name = userForCreationDto.Name,
//             Surname = userForCreationDto.Surname,
//             Email = userForCreationDto.Email,
//             Password = userForCreationDto.Password
//         };
//
//         unitOfWork.UserRepository.Insert(user);
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//
//         return new UserDto
//         {
//             Id = user.Id,
//             Name = user.Name,
//             Surname = user.Surname,
//             Email = user.Email,
//             Password = user.Password
//         };
//     }
//
//     public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         var user = await unitOfWork.UserRepository
//            .GetByIdAsync(id, cancellationToken)
//            ?? throw new UserNotFoundException(id);
//
//         unitOfWork.UserRepository.Remove(user);
//
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//     }
//
//     public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
//     {
//         var users = await unitOfWork.UserRepository
//             .GetAllAsync(cancellationToken);
//
//         var teachersDtos = new List<UserDto>();
//
//         foreach (var user in users)
//         {
//             teachersDtos.Add(new UserDto
//             {
//                 Id = user.Id,
//                 Name = user.Name,
//                 Surname = user.Surname,
//                 Email = user.Email,
//                 Password = user.Password
//             });
//         }
//
//         return teachersDtos;
//     }
//
//     public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         var user = await unitOfWork.UserRepository
//            .GetByIdAsync(id, cancellationToken)
//             ?? throw new UserNotFoundException(id);
//
//         return new UserDto
//         {
//             Id = user.Id,
//             Name = user.Name,
//             Surname = user.Surname,
//             Email = user.Email,
//             Password = user.Password
//         };
//     }
//
//     public async Task UpdateAsync(Guid id, UserForUpdateDto userForUpdateDto, CancellationToken cancellationToken = default)
//     {
//         var user = await unitOfWork.UserRepository
//            .GetByIdAsync(id, cancellationToken)
//             ?? throw new UserNotFoundException(id);
//
//         user.Name = userForUpdateDto.Name;
//         user.Surname = userForUpdateDto.Surname;
//         user.Email = userForUpdateDto.Email;
//         user.Password = userForUpdateDto.Password;
//
//         await unitOfWork.SaveChangesAsync(cancellationToken);
//     }
// }