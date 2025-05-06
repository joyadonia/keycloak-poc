using Microsoft.AspNetCore.Authentication;

namespace Keycloak.Poc
{
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetRefreshTokenAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            return authenticateResult.Properties?.GetTokenValue("access_token");
        }

        public async Task<string> GetRefreshTokenAsync()
        {
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            return authenticateResult.Properties?.GetTokenValue("refresh_token");
        }

    }
}
