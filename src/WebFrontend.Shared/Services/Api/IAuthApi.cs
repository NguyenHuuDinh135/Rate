using Refit;
using WebFrontend.Shared.Models.Auth;

namespace WebFrontend.Shared.Services.Api;

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<TokenResponseDto> LoginAsync([Body] LoginRequest payload);

    [Post("/api/auth/register")]
    Task<WebFrontend.Shared.Models.Common.ApiResponse<int>> RegisterAsync([Body] RegisterRequest payload);

    [Post("/api/auth/refresh")]
    Task<TokenResponseDto> RefreshTokenAsync();

    [Post("/api/auth/logout")]
    Task LogoutAsync();
}
