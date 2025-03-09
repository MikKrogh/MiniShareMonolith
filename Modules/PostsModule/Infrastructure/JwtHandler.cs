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
}
