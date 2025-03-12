namespace PostsModule.Domain.Auth;

public interface IAuthHelper
{
    string CreateToken(DateTime? expirationDate = null, params ClaimValueHolder[] desiredClaims);
    Dictionary<string, string>? ReadClaims(string token);
}
