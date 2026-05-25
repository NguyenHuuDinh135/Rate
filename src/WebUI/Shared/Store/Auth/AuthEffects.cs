using Fluxor;
using System.Net;
using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Services.Api;
using WebUI.Shared.Services.Storage;

namespace WebUI.Shared.Store.Auth;

public class AuthEffects(IAuthApi authApi, IPermissionApi permissionApi, ITokenStorage tokenStorage)
{
    [EffectMethod]
    public async Task HandleLogin(LoginAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await authApi.LoginAsync(new LoginRequest(action.Email, action.Password));
            
            List<string> permissions = [];
            try
            {
                permissions = await permissionApi.GetMyPermissionsAsync();
            }
            catch
            {
                // Permissions are refreshed later; login should not expose permission API details to the user.
            }

            var userWithPerms = result with { Permissions = permissions };

            dispatcher.Dispatch(new LoginSuccessAction(userWithPerms));
        }
        catch (ApiException ex)
        {
            dispatcher.Dispatch(new LoginFailureAction(MapLoginError(ex.StatusCode)));
        }
        catch (Exception ex)
        {
            _ = ex;
            dispatcher.Dispatch(new LoginFailureAction("Không thể đăng nhập lúc này. Vui lòng thử lại."));
        }
    }

    [EffectMethod]
    public async Task HandleLogout(LogoutAction action, IDispatcher dispatcher)
    {
        try
        {
            await authApi.LogoutAsync();
        }
        catch
        {
            // Ignore error on logout call
        }
        finally
        {
            await tokenStorage.ClearTokenAsync();
        }
    }

    [EffectMethod]
    public async Task HandleInitialize(InitializeAuthAction action, IDispatcher dispatcher)
    {
        try
        {
            var permissions = await permissionApi.GetMyPermissionsAsync();
            dispatcher.Dispatch(new LoginSuccessAction(new TokenResponseDto(null, null, null, null, null, permissions)));
        }
        catch
        {
            // Not authenticated, do nothing
        }
    }

    private static string MapLoginError(HttpStatusCode statusCode)
        => statusCode switch
        {
            HttpStatusCode.Unauthorized => "Email hoặc mật khẩu không đúng.",
            (HttpStatusCode)429 => "Bạn thao tác quá nhanh. Vui lòng thử lại sau ít phút.",
            HttpStatusCode.BadRequest => "Thông tin đăng nhập không hợp lệ.",
            _ => "Không thể đăng nhập lúc này. Vui lòng thử lại."
        };
}
