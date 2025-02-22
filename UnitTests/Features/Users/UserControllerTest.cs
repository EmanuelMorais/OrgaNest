using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Auth;
using OrgaNestApi.Features.Users;
using OrgaNestApi.Infrastructure.Auth;

namespace UnitTests.Features.Users;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IUserService> _mockUserService;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockRoleManager =
            new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null,
            null, null, null, null, null, null, null);
        _controller = new UserController(
            _mockUserService.Object,
            _mockRoleManager.Object,
            _mockUserManager.Object
        );
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "john.doe@example.com",
            Password = "Password123",
            Name = "John Doe"
        };

        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = createUserDto.Name,
            Email = createUserDto.Email
        };

        var identityResult = IdentityResult.Success;
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        _mockUserService.Setup(s => s.CreateUserAsync(createUserDto))
            .ReturnsAsync(userDto);

        _mockRoleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateUser(createUserDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.ActionName.Should().Be("GetUser");
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "john.doe@example.com",
            Password = "Password123",
            Name = "John Doe"
        };

        var identityResult = IdentityResult.Failed(new IdentityError { Description = "User creation failed" });
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _controller.CreateUser(createUserDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
        var errors = badRequestResult.Value.Should().BeOfType<List<IdentityError>>().Subject;
        errors.Should().ContainSingle()
            .Which.Description.Should().Contain("User creation failed");
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            Name = "Jane Doe",
            Email = "jane.doe@example.com"
        };

        _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
            .ReturnsAsync(userDto);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Which;
        var returnedUserDto = okResult.Value.Should().BeOfType<UserDto>().Subject;
        returnedUserDto.Id.Should().Be(userId);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
            .ReturnsAsync((UserDto)null);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnListOfUsers()
    {
        // Arrange
        var mockUsers = new List<User>
        {
            new() { Id = Guid.NewGuid(), Name = "User1", Email = "user1@example.com" },
            new() { Id = Guid.NewGuid(), Name = "User2", Email = "user2@example.com" }
        };

        _mockUserService.Setup(s => s.GetAllUsersAsync())
            .ReturnsAsync(mockUsers);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Which;
        var users = okResult.Value.Should().BeOfType<List<UserDto>>().Subject;
        users.Count.Should().Be(2);
    }

    [Fact]
    public async Task CreateRole_ShouldReturnBadRequest_WhenRoleExists()
    {
        // Arrange
        var createRoleDto = new CreateRoleDto { RoleName = "Admin" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync(createRoleDto.RoleName))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateRole(createRoleDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be("Role already exists.");
    }

    [Fact]
    public async Task CreateRole_ShouldReturnOk_WhenRoleIsCreated()
    {
        // Arrange
        var createRoleDto = new CreateRoleDto { RoleName = "Admin" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync(createRoleDto.RoleName))
            .ReturnsAsync(false);
        _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.CreateRole(createRoleDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { Message = "Role created successfully!" });
    }

    [Fact]
    public async Task AssignRoleToUser_ShouldReturnOk_WhenRoleIsAssigned()
    {
        // Arrange
        var assignRoleModel = new AssignRoleModel { Email = "user1@example.com", RoleName = "Admin" };
        var applicationUser = new ApplicationUser
            { UserName = assignRoleModel.Email, Email = assignRoleModel.Email };

        _mockUserManager
            .Setup(um => um.FindByEmailAsync(assignRoleModel.Email))
            .ReturnsAsync(applicationUser);

        _mockUserManager
            .Setup(um => um.AddToRoleAsync(applicationUser, assignRoleModel.RoleName))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.AssignRoleToUser(assignRoleModel);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AssignRoleToUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var assignRoleModel = new AssignRoleModel { Email = "nonexistent@example.com", RoleName = "Admin" };
        _mockUserManager.Setup(um => um.FindByEmailAsync(assignRoleModel.Email))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _controller.AssignRoleToUser(assignRoleModel);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Which;
        notFoundResult.Value.Should().Be("User not found.");
    }
}