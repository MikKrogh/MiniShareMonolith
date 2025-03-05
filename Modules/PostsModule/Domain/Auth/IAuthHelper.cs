namespace PostsModule.Domain.Auth;

public interface IAuthHelper
{
    string GenerateToken(DateTime? expirationDate = null, params ClaimValueHolder[] desiredClaims);
}
