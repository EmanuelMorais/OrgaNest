using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrgaNestApi.Infrastructure.Auth;

namespace OrgaNestApi.Features.Auth;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Create User
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "User created successfully!" });
    }

    // Create Role
    [HttpPost("role/create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel model)
    {
        var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
        if (roleExist)
            return BadRequest("Role already exists.");

        var role = new IdentityRole { Name = model.RoleName };
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

public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized();

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
            return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleLoginCallback")
        };
        return Challenge(authenticationProperties, "Google");
    }

    [HttpGet("google-login-callback")]
    public async Task<IActionResult> GoogleLoginCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("Login");

        var result =
            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        if (result.Succeeded) return RedirectToAction("Index", "Home");

        throw new Exception($"External login provider: {info.LoginProvider}");
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateUserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateRoleModel
{
    public string RoleName { get; set; }
}

public class AssignRoleModel
{
    public string Email { get; set; }
    public string RoleName { get; set; }
}