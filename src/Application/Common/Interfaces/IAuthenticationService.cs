using backend.Application.Common.Models;

namespace backend.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<AuthTokenResult?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthTokenResult?> RefreshAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<string?> CreatePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}

