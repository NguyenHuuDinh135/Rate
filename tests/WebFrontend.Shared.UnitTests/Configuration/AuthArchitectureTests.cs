using Shouldly;
using Xunit;

namespace WebUI.Shared.UnitTests.Configuration;

public class AuthArchitectureTests
{
    private static readonly DirectoryInfo RepoRoot = FindRepoRoot();

    [Fact]
    public void AuthServiceLayer_Should_ExposeRequiredLocalStorageFlowMethods()
    {
        var authService = Read("src/WebUI/Shared/Services/Auth/IAuthService.cs");
        var stateService = Read("src/WebUI/Shared/Services/Auth/AuthStateService.cs");
        var provider = Read("src/WebUI/Shared/Services/Auth/CustomAuthenticationStateProvider.cs");

        authService.ShouldContain("Task<AuthUserDto> LoginAsync(LoginRequest request)");
        authService.ShouldContain("Task<OperationResultDto> RegisterAsync(RegisterRequest request)");
        authService.ShouldContain("Task LogoutAsync()");
        authService.ShouldContain("Task<AuthUserDto?> GetCurrentUserAsync()");
        authService.ShouldContain("Task<AuthUserDto?> RestoreSessionAsync()");

        stateService.ShouldContain("bool IsAuthenticated");
        stateService.ShouldContain("AuthUserDto? CurrentUser");
        stateService.ShouldContain("bool IsLoading");
        stateService.ShouldContain("string? ErrorMessage");
        stateService.ShouldContain("event Action? OnChange");
        stateService.ShouldContain("ClearError()");

        provider.ShouldContain("NotifyUserAuthentication(AuthUserDto user)");
        provider.ShouldContain("NotifyUserLogout()");
        provider.ShouldNotContain("JwtSecurityTokenHandler");
    }

    [Fact]
    public void TokenStorage_Should_StoreAccessAndRefreshTokensSeparately()
    {
        var tokenStorage = Read("src/WebUI/Shared/Services/Storage/ITokenStorage.cs");
        var webTokenStorage = Read("src/WebUI/Shared/Services/Storage/WebTokenStorage.cs");

        tokenStorage.ShouldContain("SetAccessTokenAsync");
        tokenStorage.ShouldContain("GetAccessTokenAsync");
        tokenStorage.ShouldContain("SetRefreshTokenAsync");
        tokenStorage.ShouldContain("GetRefreshTokenAsync");
        tokenStorage.ShouldContain("ClearAsync");

        webTokenStorage.ShouldContain("rate.access_token");
        webTokenStorage.ShouldContain("rate.refresh_token");
        webTokenStorage.ShouldNotContain("document.cookie");
    }

    [Fact]
    public void BearerTokenHandler_Should_AttachTokenAndClearSessionOnUnauthorized()
    {
        var handler = Read("src/WebUI/Shared/Services/Api/BearerTokenHandler.cs");

        handler.ShouldContain("GetAccessTokenAsync");
        handler.ShouldContain("AuthenticationHeaderValue(\"Bearer\", token)");
        handler.ShouldContain("HttpStatusCode.Unauthorized");
        handler.ShouldContain("ClearAsync");
        handler.ShouldContain("NotifyUserLogout");
        handler.ShouldContain("AuthSessionExpiredAction");
    }

    [Fact]
    public void UserApi_Should_ExposeCurrentUserEndpoint()
    {
        var api = Read("src/WebUI/Shared/Services/Api/IUserApi.cs");
        var models = Read("src/WebUI/Shared/Models/Auth/AuthModels.cs");

        api.ShouldContain("[Get(\"/api/users/me\")]");
        api.ShouldContain("Task<AuthUserDto> GetMeAsync()");
        models.ShouldContain("public record AuthUserDto(string Id, string UserName, string Email);");
    }

    private static string Read(string relativePath)
        => File.ReadAllText(Path.Combine(RepoRoot.FullName, relativePath));

    private static DirectoryInfo FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Directory.Packages.props")))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
