using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthController(IJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // For demonstration, we use a dummy user check.
        // In a real app, you would validate credentials against a database.
        if (request.Username == "admin" && request.Password == "password")
        {
            var token = _tokenGenerator.GenerateToken("1", request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}

public record LoginRequest(string Username, string Password);
