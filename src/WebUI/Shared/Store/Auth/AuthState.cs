using WebUI.Shared.Models.Auth;

namespace WebUI.Shared.Store.Auth;

public record AuthState(
    bool IsAuthenticated,
    bool IsLoading,
    string? Error,
    TokenResponseDto? User,
    List<string> Permissions);

public record LoginAction(string Email, string Password);
public record LoginSuccessAction(TokenResponseDto User);
public record LoginFailureAction(string Error);
public record LogoutAction();
public record InitializeAuthAction();
public record AuthSessionExpiredAction(string Error);
