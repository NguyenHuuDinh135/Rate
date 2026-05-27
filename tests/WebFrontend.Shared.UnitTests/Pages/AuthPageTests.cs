using Shouldly;
using Xunit;

namespace WebUI.Shared.UnitTests.Pages;

public class AuthPageTests
{
    private static readonly string[] AuthPageFiles =
    [
        "Login.razor",
        "Register.razor",
        "ForgotPassword.razor",
        "VerifyOtp.razor",
        "ResetPassword.razor"
    ];

    [Fact]
    public void AuthPages_Should_Use_AuthLayout()
    {
        foreach (var page in AuthPageFiles)
        {
            var source = ReadRepoFile("src", "WebUI", "Shared", "Pages", "Auth", page);

            source.ShouldContain("@layout AuthLayout");
        }
    }

    [Fact]
    public void AuthPages_Should_Not_Use_MudBlazor()
    {
        foreach (var page in AuthPageFiles)
        {
            var source = ReadRepoFile("src", "WebUI", "Shared", "Pages", "Auth", page);

            source.ShouldNotContain("@using MudBlazor");
            source.ShouldNotContain("MudButton");
            source.ShouldNotContain("MudTextField");
            source.ShouldNotContain("MudAlert");
            source.ShouldNotContain("MudLink");
            source.ShouldNotContain("ISnackbar");
        }
    }

    [Fact]
    public void ThemeToggle_Should_Use_Lucide_Icons_And_Persist_With_RateTheme()
    {
        var themeToggle = ReadRepoFile("src", "WebUI", "Shared", "Components", "UI", "ThemeToggle.razor");
        var appScript = ReadRepoFile("src", "WebUI", "Server", "wwwroot", "js", "app-animations.js");
        var app = ReadRepoFile("src", "WebUI", "Server", "Components", "App.razor");

        themeToggle.ShouldContain("<Blazicon");
        themeToggle.ShouldContain("Lucide.Sun");
        themeToggle.ShouldContain("Lucide.Moon");
        themeToggle.ShouldNotContain("<svg");
        themeToggle.ShouldContain("rateTheme.init");
        themeToggle.ShouldContain("rateTheme.set");

        appScript.ShouldContain("key: \"rate-theme\"");
        appScript.ShouldContain("const root = document.documentElement");
        appScript.ShouldContain("root.classList.toggle(\"dark\"");
        appScript.ShouldContain("root.dataset.theme");

        app.ShouldContain("localStorage.getItem(\"rate-theme\")");
        app.ShouldContain("document.documentElement.classList.toggle(\"dark\", isDarkMode)");
    }

    [Fact]
    public void Tailwind_Should_Use_Class_Based_Dark_Mode_For_ThemeToggle()
    {
        var tailwindConfig = ReadRepoFile("tailwind.config.js");

        tailwindConfig.ShouldContain("darkMode: 'class'");
    }

    [Fact]
    public void AuthThemeCss_Should_Override_Auth_Surface_With_DataTheme()
    {
        var root = FindRepoRoot();
        var authCssPath = Path.Combine(root, "src", "WebUI", "Server", "wwwroot", "auth.css");
        var app = ReadRepoFile("src", "WebUI", "Server", "Components", "App.razor");
        var authLayout = ReadRepoFile("src", "WebUI", "Shared", "Layout", "AuthLayout.razor");

        File.Exists(authCssPath).ShouldBeTrue();

        var authCss = File.ReadAllText(authCssPath);

        app.ShouldContain("@Assets[\"auth.css\"]");
        authLayout.ShouldContain("rate-auth-shell");
        authLayout.ShouldContain("rate-auth-header");
        authCss.ShouldContain("html[data-theme=\"light\"] .rate-auth-shell");
        authCss.ShouldContain("html[data-theme=\"dark\"] .rate-auth-shell");
        authCss.ShouldContain("html[data-theme=\"dark\"] .rate-auth-card");
        authCss.ShouldContain("html[data-theme=\"light\"] .rate-auth-input");
        authCss.ShouldContain("html[data-theme=\"dark\"] .rate-auth-input");
    }

    [Fact]
    public void Auth_Register_Contract_Should_Match_Backend_Result()
    {
        var models = ReadRepoFile("src", "WebUI", "Shared", "Models", "Auth", "AuthModels.cs");
        var api = ReadRepoFile("src", "WebUI", "Shared", "Services", "Api", "IAuthApi.cs");

        models.ShouldContain("public record RegisterRequest(string FullName, string Email, string Password);");
        api.ShouldContain("Task<OperationResultDto> RegisterAsync([Body] RegisterRequest payload);");
        api.ShouldNotContain("ApiResponse<int>");
    }

    [Fact]
    public void AuthEffects_Should_Not_Surface_Raw_Exception_Messages()
    {
        var source = ReadRepoFile("src", "WebUI", "Shared", "Store", "Auth", "AuthEffects.cs");

        source.ShouldNotContain("new LoginFailureAction(ex.Message)");
    }

    private static string ReadRepoFile(params string[] segments)
    {
        var root = FindRepoRoot();
        return File.ReadAllText(Path.Combine([root, .. segments]));
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Rate.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate Rate repository root.");
    }
}
