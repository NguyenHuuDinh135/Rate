using Fluxor;

namespace WebUI.Shared.Store.Auth;

public static class AuthReducers
{
    [ReducerMethod]
    public static AuthState OnLogin(AuthState state, LoginAction action)
        => state with { IsLoading = true, Error = null };

    [ReducerMethod]
    public static AuthState OnLoginSuccess(AuthState state, LoginSuccessAction action)
        => state with { 
            IsLoading = false, 
            IsAuthenticated = true, 
            User = action.User, 
            Permissions = action.User.Permissions ?? new List<string>(),
            Error = null 
        };

    [ReducerMethod]
    public static AuthState OnLoginFailure(AuthState state, LoginFailureAction action)
        => state with { IsLoading = false, IsAuthenticated = false, User = null, Permissions = new List<string>(), Error = action.Error };

    [ReducerMethod]
    public static AuthState OnLogout(AuthState state, LogoutAction action)
        => new AuthState(false, false, null, null, new List<string>());

    [ReducerMethod]
    public static AuthState OnSessionExpired(AuthState state, AuthSessionExpiredAction action)
        => new AuthState(false, false, action.Error, null, new List<string>());
}

public class AuthFeature : Feature<AuthState>
{
    public override string GetName() => "Auth";
    protected override AuthState GetInitialState() => new AuthState(false, false, null, null, new List<string>());
}
