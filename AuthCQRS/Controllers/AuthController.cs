using AuthCQRS.Application.Common.Interfaces;
using AuthCQRS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthCQRS.Web.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController (IIdentityService identityService,
        UserManager<ApplicationUser> userManager)
    {
        _identityService = identityService;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register (string username, string password, string role = "User")
    {
        var (result, userId) = await _identityService.CreateUserAsync(username, password);

        if (result != "Succeeded")
        {
            return BadRequest(result);
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return BadRequest("User registration failed");
        }

        if (!string.IsNullOrEmpty(role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, UserId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login (string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    private string GenerateJwtToken (ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("6143d04c3a714f6c899d555b7d14c75b"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "AuthCQRS API",
            audience: "Swagger-Client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
