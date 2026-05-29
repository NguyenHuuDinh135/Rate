using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<WebUI.Shared.Models.Common.ApiResponse<TokenResponseDto>> LoginAsync([Body] LoginRequest payload);

    [Post("/api/auth/register")]
    Task<OperationResultDto> RegisterAsync([Body] RegisterRequest payload);

    [Post("/api/auth/refresh")]
    Task<WebUI.Shared.Models.Common.ApiResponse<TokenResponseDto>> RefreshTokenAsync();

    [Post("/api/auth/logout")]
    Task LogoutAsync();

    [Post("/api/auth/forgot-password")]
    Task<ForgotPasswordResponse> ForgotPasswordAsync([Body] ForgotPasswordRequest payload);

    [Post("/api/auth/reset-password")]
    Task<OperationResultDto> ResetPasswordAsync([Body] ResetPasswordRequest payload);

    [Post("/api/auth/change-password")]
    Task<OperationResultDto> ChangePasswordAsync([Body] ChangePasswordRequest payload);
}
