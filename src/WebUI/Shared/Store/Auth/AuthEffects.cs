using Fluxor;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Services.Auth;

namespace WebUI.Shared.Store.Auth;

public class AuthEffects(AuthStateService authStateService)
{
    [EffectMethod]
    public async Task HandleLogin(LoginAction action, IDispatcher dispatcher)
    {
        var user = await authStateService.LoginAsync(new LoginRequest(action.Email, action.Password));
        if (user is null)
        {
            dispatcher.Dispatch(new LoginFailureAction(authStateService.ErrorMessage ?? "Không thể đăng nhập lúc này. Vui lòng thử lại."));
            return;
        }

        dispatcher.Dispatch(new LoginSuccessAction(ToTokenResponse(user)));
    }

    [EffectMethod]
    public async Task HandleLogout(LogoutAction action, IDispatcher dispatcher)
    {
        await authStateService.LogoutAsync();
    }

    [EffectMethod]
    public async Task HandleInitialize(InitializeAuthAction action, IDispatcher dispatcher)
    {
        var user = await authStateService.RestoreSessionAsync();
        if (user is not null)
        {
            dispatcher.Dispatch(new LoginSuccessAction(ToTokenResponse(user)));
        }
    }

    private static TokenResponseDto ToTokenResponse(AuthUserDto user)
        => new(null, null, user.UserName, user.Email, null, [], user.Roles);
}
