using OrgaNestApi.Common.Domain;

namespace OrgaNestApi.Features.Users;

public interface IUserService
{
    Task<User> CreateUserAsync(string name, string email);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<List<User>> GetAllUsersAsync();
}
