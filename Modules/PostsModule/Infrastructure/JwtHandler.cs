using Microsoft.IdentityModel.Tokens;
using PostsModule.Domain.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostsModule.Infrastructure;


public sealed class JwtHandler : IAuthHelper
{
    public SymmetricSecurityKey securityKey { get; init; }

    public JwtHandler(IConfiguration configuration)
    {
        string secret = configuration["JwtSecret"] ?? throw new Exception("Cannot generator jwt secret");
        securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    public string CreateToken(DateTime? expirationDate = null, params ClaimValueHolder[] desiredClaims)
    {
        List<Claim> claims = new();
        foreach (var item in desiredClaims)
        {
            claims.Add(new Claim(item.Key, item.Value, item.ValueType));
        }
        var jtwToken = CreateToken(claims, expirationDate ?? DateTime.Now.AddMinutes(5));
        return new JwtSecurityTokenHandler().WriteToken(jtwToken);
    }

    private JwtSecurityToken CreateToken(List<Claim> claims, DateTime expirationDate) =>
        new JwtSecurityToken(
            issuer: "MiniShare",
            claims: claims,
            expires: expirationDate,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "MiniShare",
                ValidateAudience = false, // Set to true if you have an audience
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero // Optional: Set to zero to avoid allowing expired tokens due to clock drift
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null; // Invalid token
        }
    }
    public Dictionary<string, string>? ReadClaims(string token)
    {
        var principal = ValidateToken(token);
        return principal?.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}
