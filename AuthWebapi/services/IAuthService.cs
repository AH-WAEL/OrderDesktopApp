
using AuthWebapi.Entities;
using AuthWebapi.Models;

namespace AuthWebapi.services
{
    public interface IAuthService 
    {

        public Task<registerResponseDto?> RegisterAsync(userRegisterDTO request);
        public Task<TokenResponseDto?> LoginAsync(loginDto request);
        public Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);

    }
}