using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Services;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RefreshTokenService _refreshService;
    private readonly IConfiguration _config;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RefreshTokenService refreshService, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _refreshService = refreshService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null)
            return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded) return Unauthorized();

        // create access token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key") ?? "dev-secret-key-please-change");
        var ttlMinutes = _config.GetValue<int?>("Jwt:AccessTokenLifetimeMinutes") ?? 15;
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName ?? string.Empty) };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ttlMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // create refresh token
        var refreshEntity = await _refreshService.CreateRefreshTokenAsync(user.Id, clientId: "banking-client", device: null, lifetime: req.RememberMe ? TimeSpan.FromDays(90) : TimeSpan.FromDays(30));
        var refreshToken = refreshEntity.Device ?? string.Empty; // device field used to carry raw token back

        var resp = new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresIn = ttlMinutes * 60 };
        return Ok(resp);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var existing = await _refreshService.FindByTokenAsync(req.RefreshToken);
        if (existing == null || !existing.IsActive)
            return Unauthorized();

        // rotate
        var newTokenEntity = await _refreshService.RotateAsync(existing, TimeSpan.FromDays(30));
        var raw = newTokenEntity.Device ?? string.Empty;

        // create new access token for user
        var user = await _userManager.FindByIdAsync(existing.UserId);
        if (user == null) return Unauthorized();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key") ?? "dev-secret-key-please-change");
        var ttlMinutes = _config.GetValue<int?>("Jwt:AccessTokenLifetimeMinutes") ?? 15;
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName ?? string.Empty) };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ttlMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var resp = new TokenResponse { AccessToken = accessToken, RefreshToken = raw, ExpiresIn = ttlMinutes * 60 };
        return Ok(resp);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest? req)
    {
        if (req != null && !string.IsNullOrWhiteSpace(req.RefreshToken))
        {
            var existing = await _refreshService.FindByTokenAsync(req.RefreshToken);
            if (existing != null)
            {
                await _refreshService.RevokeAsync(existing);
            }
        }
        else
        {
            // revoke all tokens for current user
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sub != null)
            {
                var tokens = await HttpContext.RequestServices.GetRequiredService<AuthDbContext>().RefreshTokens.Where(x => x.UserId == sub && x.RevokedAt == null).ToListAsync();
                foreach (var t in tokens) t.RevokedAt = DateTimeOffset.UtcNow;
                await HttpContext.RequestServices.GetRequiredService<AuthDbContext>().SaveChangesAsync();
            }
        }

        return NoContent();
    }
}
