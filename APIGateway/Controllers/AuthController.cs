using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIGateway.Models;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration configuration)
    {
        _config = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validación simple (en producción, validar contra base de datos)
        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = GenerateJwtToken(request.Username);
            return Ok(new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(
                    GetExpirationMinutes())
            });
        }

        return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
    }

    private string GenerateJwtToken(string username)
    {
        var jwtKey = GetJwtKey();
        var jwtIssuer = GetJwtIssuer();
        var jwtAudience = GetJwtAudience();
        var expirationMinutes = GetExpirationMinutes();

        var credentials = CreateCredentials(jwtKey);
        var claims = BuildClaims(username);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetExpirationMinutes()
    {
        return int.Parse(_config["Jwt:ExpirationMinutes"] ?? "60");
    }

    private string GetJwtKey()
    {
        return _config["Jwt:Key"] ?? "MiClaveSecretaSuperSeguraParaJWT2024!";
    }

    private string GetJwtIssuer()
    {
        return _config["Jwt:Issuer"] ?? "APIGateway";
    }

    private string GetJwtAudience()
    {
        return _config["Jwt:Audience"] ?? "APIEmpleados";
    }

    private static SigningCredentials CreateCredentials(string jwtKey)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private static Claim[] BuildClaims(string username)
    {
        return new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username)
        };
    }
}
