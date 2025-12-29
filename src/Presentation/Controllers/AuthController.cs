using Application.Common.Interfaces;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IApplicationDbContext _context;

    public AuthController(IJwtTokenGenerator tokenGenerator, IApplicationDbContext context)
    {
        _tokenGenerator = tokenGenerator;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

        if (user == null)
            return Unauthorized("Invalid username or password.");

        var accessToken = _tokenGenerator.GenerateToken(user.Id.ToString(), user.Username, user.Role);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(accessToken, refreshToken));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token.");

        var newAccessToken = _tokenGenerator.GenerateToken(user.Id.ToString(), user.Username, user.Role);
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(newAccessToken, newRefreshToken));
    }
}

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken);
