using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Users;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(string name, string email);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<List<User>> GetAllUsersAsync();
}