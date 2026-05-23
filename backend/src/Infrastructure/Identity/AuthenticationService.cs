using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace backend.Infrastructure.Identity;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IRefreshTokenStore refreshTokenStore,
    IOneTimeTokenService oneTimeTokenService,
    IOptions<JwtSettings> jwtSettings)
    : IAuthenticationService
{
    private readonly JwtSettings _settings = jwtSettings.Value;
    private static readonly TimeSpan PasswordResetTokenTtl = TimeSpan.FromMinutes(15);

    public async Task<AuthTokenResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[Login] Attempting login for email: {email}");
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            Console.WriteLine($"[Login] User not found for email: {email}");
            return null;
        }

        var ok = await userManager.CheckPasswordAsync(user, password);
        if (!ok)
        {
            Console.WriteLine($"[Login] Password check failed for user: {email}");
            // Check if user is locked out or email not confirmed if those are required
            var isLocked = await userManager.IsLockedOutAsync(user);
            if (isLocked) Console.WriteLine($"[Login] User {email} is locked out.");
            return null;
        }

        Console.WriteLine($"[Login] Login successful for user: {email}");
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateAccessToken(
            user.Id,
            user.Email ?? string.Empty,
            user.UserName ?? string.Empty,
            roles);

        var refreshToken = jwtService.GenerateRefreshToken();
        await refreshTokenStore.RevokeAsync(user.Id, cancellationToken);
        await refreshTokenStore.StoreAsync(
            user.Id,
            refreshToken,
            TimeSpan.FromDays(_settings.RefreshTokenExpiryDays),
            cancellationToken);

        return new AuthTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public async Task<AuthTokenResult?> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        string? userId = null;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            userId = jwtService.ValidateAccessToken(accessToken)?.ToString() 
                     ?? jwtService.GetUserIdFromExpiredToken(accessToken)?.ToString();
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            // Try to get userId from refreshToken mapping if accessToken is missing or invalid
            userId = await refreshTokenStore.GetUserIdAsync(refreshToken, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var valid = await refreshTokenStore.ValidateAsync(userId, refreshToken, cancellationToken);
        if (!valid)
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = jwtService.GenerateAccessToken(
            user.Id,
            user.Email ?? string.Empty,
            user.UserName ?? string.Empty,
            roles);

        var newRefreshToken = jwtService.GenerateRefreshToken();
        await refreshTokenStore.StoreAsync(
            user.Id,
            newRefreshToken,
            TimeSpan.FromDays(_settings.RefreshTokenExpiryDays),
            cancellationToken);

        return new AuthTokenResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };
    }

    public async Task<string?> CreatePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var resetToken = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await oneTimeTokenService.StoreAsync("pwd-reset", email.ToLowerInvariant(), resetToken, PasswordResetTokenTtl, cancellationToken);
        return resetToken;
    }

    public async Task<bool> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return false;
        }

        var consumed = await oneTimeTokenService.ConsumeAsync("pwd-reset", email.ToLowerInvariant(), resetToken, cancellationToken);
        if (!consumed)
        {
            return false;
        }

        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return false;
        }

        var addResult = await userManager.AddPasswordAsync(user, newPassword);
        return addResult.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }
}

