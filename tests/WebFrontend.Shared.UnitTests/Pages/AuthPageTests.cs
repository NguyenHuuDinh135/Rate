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
