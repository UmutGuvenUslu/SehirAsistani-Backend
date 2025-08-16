using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SehirAsistanim.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
    private readonly IConfiguration _configuration;

    #region Constructor
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    #endregion

    #region JWT Token Üret
    public Task<string> GenerateJwtToken(Kullanici user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var role = user.Rol.ToString() ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Isim} {user.Soyisim}"),
            new Claim(ClaimTypes.Role, role), // Asıl rol (ör. SağlıkİşleriBirimi)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Eğer rol "Birimi" ile bitiyorsa, ek olarak BirimAdmin rolü ekle
        if (role.EndsWith("Birimi", StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, "BirimAdmin"));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
    #endregion
}
