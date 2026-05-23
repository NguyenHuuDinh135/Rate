using Fluxor;
using WebFrontend.Shared.Models.Auth;
using WebFrontend.Shared.Services.Api;
using WebFrontend.Shared.Services.Storage;

namespace WebFrontend.Shared.Store.Auth;

public class AuthEffects(IAuthApi authApi, IPermissionApi permissionApi, ITokenStorage tokenStorage)
{
    [EffectMethod]
    public async Task HandleLogin(LoginAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await authApi.LoginAsync(new LoginRequest(action.Email, action.Password));
            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                await tokenStorage.SetTokenAsync(result.AccessToken);

                // Fetch permissions immediately after login
                var permissions = await permissionApi.GetMyPermissionsAsync();
                var userWithPerms = result with { Permissions = permissions };

                dispatcher.Dispatch(new LoginSuccessAction(userWithPerms));
            }
            else
            {
                dispatcher.Dispatch(new LoginFailureAction("Invalid response from server."));
            }
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoginFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleLogout(LogoutAction action, IDispatcher dispatcher)
    {
        try
        {
            await authApi.LogoutAsync();
        }
        finally
        {
            await tokenStorage.ClearTokenAsync();
        }
    }

    [EffectMethod]
    public async Task HandleInitialize(InitializeAuthAction action, IDispatcher dispatcher)
    {
        var token = await tokenStorage.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var permissions = await permissionApi.GetMyPermissionsAsync();
                dispatcher.Dispatch(new LoginSuccessAction(new TokenResponseDto(token, null, null, null, permissions)));
            }
            catch
            {
                await tokenStorage.ClearTokenAsync();
            }
        }
    }
}
