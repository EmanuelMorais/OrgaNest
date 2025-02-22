using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Users;
using OrgaNestApi.Infrastructure.Database;
using FluentAssertions;

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

        public void Dispose()
        {
            _context?.Dispose();
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

            result.Should().NotBeNull();
            result.Name.Should().Be(createUserDto.Name);
            result.Email.Should().Be(createUserDto.Email);
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

            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Name.Should().Be("Jane Doe");
            result.Email.Should().Be("jane.doe@example.com");
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var result = await _service.GetUserByIdAsync(userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var result = await _service.GetAllUsersAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            var mockUsers = new List<User>
            {
                new() { Id = Guid.NewGuid(), Name = "User1", Email = "user1@example.com" },
                new() { Id = Guid.NewGuid(), Name = "User2", Email = "user2@example.com" }
            };

            await _context.Users.AddRangeAsync(mockUsers);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllUsersAsync();

            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenDatabaseIsEmpty()
        {
            var userId = Guid.NewGuid();

            var result = await _service.GetUserByIdAsync(userId);

            result.Should().BeNull();
        }
    }
}
