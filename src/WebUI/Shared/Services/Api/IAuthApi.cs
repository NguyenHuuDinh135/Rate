using Refit;
using WebUI.Shared.Models.Auth;

namespace WebUI.Shared.Services.Api;

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<TokenResponseDto> LoginAsync([Body] LoginRequest payload);

    [Post("/api/auth/register")]
    Task<WebUI.Shared.Models.Common.ApiResponse<int>> RegisterAsync([Body] RegisterRequest payload);

    [Post("/api/auth/refresh")]
    Task<TokenResponseDto> RefreshTokenAsync();

    [Post("/api/auth/logout")]
    Task LogoutAsync();
}
