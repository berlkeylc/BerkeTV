using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoCatalog.Identity.Data;
using VideoCatalog.Identity.Entities;

namespace VideoCatalog.Identity.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IdentityDbContext _dbContext;

    public AuthController(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
        {
            return BadRequest(new { error = "Username already exists" });
        }

        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return BadRequest(new { error = "Email already exists" });
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "User registered successfully", userId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest(new { error = "Invalid credentials" });
        }

        // Return token endpoint URL for the client to use
        return Ok(new { 
            token_endpoint = "/connect/token",
            message = "Use the token endpoint with password grant to get your JWT token"
        });
    }
}

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Username, string Password);
