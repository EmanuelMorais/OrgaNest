using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Users;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto userDto);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<List<User>> GetAllUsersAsync();
}