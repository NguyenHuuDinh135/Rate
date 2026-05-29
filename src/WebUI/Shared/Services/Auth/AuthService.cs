using System.Net;
using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Storage;

namespace WebUI.Shared.Services.Auth;

public sealed class AuthService(
    IAuthApi authApi,
    IUserApi userApi,
    ITokenStorage tokenStorage) : IAuthService
{
    public async Task<AuthUserDto> LoginAsync(LoginRequest request)
    {
        var token = await authApi.LoginAsync(request);

        if (string.IsNullOrWhiteSpace(token.Body.AccessToken))
        {
            throw new InvalidOperationException("Login response did not include an access token.");
        }

        await tokenStorage.SetAccessTokenAsync(token.Body.AccessToken);

        if (!string.IsNullOrWhiteSpace(token.Body.RefreshToken))
        {
            await tokenStorage.SetRefreshTokenAsync(token.Body.RefreshToken);
        }

        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            await tokenStorage.ClearAsync();
            throw new UnauthorizedAccessException("Unable to restore authenticated user.");
        }

        return user;
    }

    public async Task<OperationResultDto> RegisterAsync(RegisterRequest request)
    {
        return await authApi.RegisterAsync(request);
    }

    public async Task LogoutAsync()
    {
        try
        {
            await authApi.LogoutAsync();
        }
        catch
        {
            // Client state is cleared even when the server-side revoke call cannot complete.
        }
        finally
        {
            await tokenStorage.ClearAsync();
        }
    }

    public async Task<AuthUserDto?> GetCurrentUserAsync()
    {
        try
        {
            return await userApi.GetMeAsync();
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public async Task<AuthUserDto?> RestoreSessionAsync()
    {
        var accessToken = await tokenStorage.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            await tokenStorage.ClearAsync();
        }

        return user;
    }
}
