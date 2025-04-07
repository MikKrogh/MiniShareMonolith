using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ReverseProxy.Yarp;
public class FireBaseJwtValidator
{
    private const string _issuer = "https://securetoken.google.com/userservice-37984";
    private const string _audience = "userservice-37984";
    private readonly ILogger<FireBaseJwtValidator> logger;
    private DateTime _cacheExpiration;
    private List<X509SecurityKey> keys = new();
    public FireBaseJwtValidator(ILogger<FireBaseJwtValidator> logger)
    {
        this.logger = logger;
    }

    public  async Task<bool?> ValidateTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var validationParameters = await GetValidationParameters();
        try
        {
            handler.ValidateToken(token, validationParameters, out _);
            return true;            
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to validate a jwtToken.", ex);
            return false;
        }
    }

    public  async Task<TokenValidationParameters> GetValidationParameters()
    {
        var keys = await GetSecurityKeys();
        return new TokenValidationParameters
        {
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKeys = keys,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private async Task<IEnumerable<SecurityKey>> GetSecurityKeys()
    {
        if (DateTime.UtcNow.AddSeconds(5) > _cacheExpiration || keys?.Any() is false)        
            await UpdateKeys();        
        return keys;
    }

    private async Task UpdateKeys()
    {
        var client = new HttpClient();
        var reponse = await client.GetAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com");
        if (reponse is null || !reponse.IsSuccessStatusCode) throw new Exception("Failed to access google public key service");

        UpdateCacheExpiration(reponse?.Headers?.CacheControl?.MaxAge);
        var content = await reponse!.Content.ReadAsStringAsync();
        SetSecurityKeys(content);
    }

    private void SetSecurityKeys(string? content)
    {
        if (string.IsNullOrEmpty(content))
            throw new Exception("Content is null or empty");
        var certificates = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        if (certificates == null || !certificates.Any()) throw new Exception("could not deserialize content");

        foreach (var certificate in certificates)
        {
            try
            {
                byte[] certificateValueAsBytes = Encoding.ASCII.GetBytes(certificate.Value);
                var key = new X509SecurityKey(new X509Certificate2(certificateValueAsBytes), certificate.Key);
                keys.Add(key);
            }
            catch (Exception e)
            {
                throw new Exception("could not transform seralized certificate into binary", e);
            }
        }
    }
    private void UpdateCacheExpiration(TimeSpan? timeUntillExpire)
    {
        if (timeUntillExpire is null) throw new Exception("Cache expiration time is null");
        var currentTime = DateTime.UtcNow;
        _cacheExpiration = (currentTime + timeUntillExpire.Value);
    }
}
