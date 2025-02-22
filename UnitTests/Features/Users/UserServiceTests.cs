using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Users;
using OrgaNestApi.Infrastructure.Database;

namespace UnitTests.Features.Users
{
    [TestSubject(typeof(UserService))]
    public class UserServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new UserService(_context);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnUserDto_WhenUserIsCreated()
        {
            var createUserDto = new CreateUserDto
            {
                Name = "John Doe",
                Email = "john.doe@example.com"
            };

            var result = await _service.CreateUserAsync(createUserDto);

            Assert.NotNull(result);
            Assert.Equal(createUserDto.Name, result.Name);
            Assert.Equal(createUserDto.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserDto_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "Jane Doe",
                Email = "jane.doe@example.com"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Jane Doe", result.Name);
            Assert.Equal("jane.doe@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var result = await _service.GetUserByIdAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var result = await _service.GetAllUsersAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            var mockUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "User1", Email = "user1@example.com" },
                new User { Id = Guid.NewGuid(), Name = "User2", Email = "user2@example.com" }
            };

            await _context.Users.AddRangeAsync(mockUsers);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllUsersAsync();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenDatabaseIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _service.GetUserByIdAsync(userId);

            Assert.Null(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
