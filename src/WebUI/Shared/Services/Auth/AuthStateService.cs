using System.Net;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Auth;

public sealed class AuthStateService(
    IAuthService authService,
    AuthenticationStateProvider authenticationStateProvider)
{
    public bool IsAuthenticated => CurrentUser is not null;

    public bool IsInRole(string role)
        => CurrentUser?.Roles?.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;

    public AuthUserDto? CurrentUser { get; private set; }
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? OnChange;

    public async Task<AuthUserDto?> LoginAsync(LoginRequest request)
    {
        SetLoading(true);
        ClearError(notify: false);

        try
        {
            var user = await authService.LoginAsync(request);
            SetAuthenticated(user);
            return user;
        }
        catch (ApiException ex)
        {
            MarkLoggedOut(MapLoginError(ex.StatusCode));
            return null;
        }
        catch
        {
            MarkLoggedOut("Không thể đăng nhập lúc này. Vui lòng thử lại.");
            return null;
        }
        finally
        {
            SetLoading(false);
        }
    }

    public async Task<OperationResultDto> RegisterAsync(RegisterRequest request)
    {
        SetLoading(true);
        ClearError(notify: false);

        try
        {
            var result = await authService.RegisterAsync(request);
            if (!result.Succeeded)
            {
                ErrorMessage = MapRegisterError(result.Errors);
                NotifyChanged();
            }

            return result;
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.StatusCode is HttpStatusCode.BadRequest
                ? "Thông tin đăng ký không hợp lệ hoặc email đã được sử dụng."
                : "Không thể tạo tài khoản lúc này. Vui lòng thử lại.";
            NotifyChanged();
            return new OperationResultDto(false, [ErrorMessage!]);
        }
        catch
        {
            ErrorMessage = "Không thể tạo tài khoản lúc này. Vui lòng thử lại.";
            NotifyChanged();
            return new OperationResultDto(false, [ErrorMessage!]);
        }
        finally
        {
            SetLoading(false);
        }
    }

    public async Task LogoutAsync()
    {
        SetLoading(true);

        try
        {
            await authService.LogoutAsync();
        }
        finally
        {
            MarkLoggedOut();
            SetLoading(false);
        }
    }

    public async Task<AuthUserDto?> RestoreSessionAsync()
    {
        SetLoading(true);
        ClearError(notify: false);

        try
        {
            var user = await authService.RestoreSessionAsync();
            if (user is null)
            {
                MarkLoggedOut();
                return null;
            }

            SetAuthenticated(user);
            return user;
        }
        catch (ApiException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized)
        {
            MarkLoggedOut();
            return null;
        }
        catch
        {
            MarkLoggedOut("Không thể khôi phục phiên đăng nhập.");
            return null;
        }
        finally
        {
            SetLoading(false);
        }
    }

    public void MarkSessionExpired(string? errorMessage = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.")
    {
        MarkLoggedOut(errorMessage);
    }

    public void ClearError()
        => ClearError(notify: true);

    private void SetAuthenticated(AuthUserDto user)
    {
        CurrentUser = user;
        ErrorMessage = null;

        if (authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserAuthentication(user);
        }

        NotifyChanged();
    }

    private void MarkLoggedOut(string? errorMessage = null)
    {
        CurrentUser = null;
        ErrorMessage = errorMessage;

        if (authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }

        NotifyChanged();
    }

    private void SetLoading(bool isLoading)
    {
        IsLoading = isLoading;
        NotifyChanged();
    }

    private void ClearError(bool notify)
    {
        ErrorMessage = null;

        if (notify)
        {
            NotifyChanged();
        }
    }

    private void NotifyChanged()
        => OnChange?.Invoke();

    private static string MapLoginError(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.Unauthorized => "Email hoặc mật khẩu không đúng.",
            (HttpStatusCode)429 => "Bạn thao tác quá nhanh. Vui lòng thử lại sau ít phút.",
            HttpStatusCode.BadRequest => "Thông tin đăng nhập không hợp lệ.",
            _ => "Không thể đăng nhập lúc này. Vui lòng thử lại."
        };

    private static string MapRegisterError(string[]? errors)
        => errors is { Length: > 0 }
            ? "Không thể tạo tài khoản với thông tin này."
            : "Không thể tạo tài khoản lúc này. Vui lòng thử lại.";
}
