using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgaNestApi.Common.Domain;
using OrgaNestApi.Features.Auth;
using OrgaNestApi.Infrastructure.Auth;
using OrgaNestApi.Infrastructure.Database;

namespace OrgaNestApi.Features.Users;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public UserController(IUserService userService, RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // Create a user
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var userAuth = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(userAuth, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var user = await _userService.CreateUserAsync(request.Name, request.Email);
        var roleName = Roles.Default.ToString();
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (roleExist) await _userManager.AddToRoleAsync(userAuth, roleName);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
    }

    // Get a user by ID
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return NotFound();
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

    // Create Role
    [HttpPost("role/create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
    {
        var roleExist = await _roleManager.RoleExistsAsync(dto.RoleName);
        if (roleExist)
            return BadRequest("Role already exists.");

        var role = new IdentityRole { Name = dto.RoleName };
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Role created successfully!" });
    }

    // Assign Role to User
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound("User not found.");

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Role assigned successfully!" });
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
    public async Task<UserDto> CreateUserAsync(string name, string email)
    {
        var user = new User
        {
            Name = name,
            Email = email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.ToDto();
    }

    // Get a user by ID
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.ToDto();
    }

    // Get all users
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}