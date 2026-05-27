using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Auth;

public interface IAuthService
{
    Task<AuthUserDto> LoginAsync(LoginRequest request);
    Task<OperationResultDto> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<AuthUserDto?> GetCurrentUserAsync();
    Task<AuthUserDto?> RestoreSessionAsync();
}
