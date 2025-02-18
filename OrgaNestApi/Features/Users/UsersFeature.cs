using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Users;
using OrgaNestApi.Infrastructure.Database;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Create a user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var user = await _userService.CreateUserAsync(request.Name, request.Email);
        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
    }

    // Get a user by ID
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        });
    }

    // Get all users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        var usersDto = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        }).ToList();

        return Ok(usersDto);
    }
}


public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    // Create a new user
    public async Task<User> CreateUserAsync(string name, string email)
    {
        var user = new User
        {
            Name = name,
            Email = email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Get a user by ID
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    // Get all users
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}