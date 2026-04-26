using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VideoCatalog.Identity.Data;

namespace VideoCatalog.Identity.Controllers;

[ApiController]
[Route("connect")]
public class TokenController : ControllerBase
{
    private readonly IdentityDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public TokenController(IdentityDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        if (request.GrantType != "password")
        {
            return BadRequest(new { error = "unsupported_grant_type" });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest(new { error = "invalid_grant", error_description = "Invalid credentials" });
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",
            expires_in = 3600
        });
    }

    private string GenerateJwtToken(Entities.ApplicationUser user)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyForJwtTokenGenerationMustBeLongEnough32Chars!";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "http://localhost:8081",
            audience: _configuration["Jwt:Audience"] ?? "api",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record TokenRequest(string GrantType, string Username, string Password, string? Scope);
