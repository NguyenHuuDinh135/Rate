using Shouldly;
using Xunit;

namespace WebUI.Shared.UnitTests.Configuration;

public class MudBlazorConfigurationTests
{
    private static readonly DirectoryInfo RepoRoot = FindRepoRoot();

    [Fact]
    public void CentralPackageManagement_Should_PinMudBlazor()
    {
        var props = Read("Directory.Packages.props");

        props.ShouldContain("""<PackageVersion Include="MudBlazor" Version="9.4.0" />""");
    }

    [Theory]
    [InlineData("src/WebUI/Shared/WebUI.Shared.csproj")]
    [InlineData("src/WebUI/Server/WebUI.Server.csproj")]
    [InlineData("src/WebUI/Client/WebUI.Client.csproj")]
    public void WebUiProjects_Should_ReferenceMudBlazor(string projectPath)
    {
        var project = Read(projectPath);

        project.ShouldContain("""<PackageReference Include="MudBlazor" />""");
    }

    [Theory]
    [InlineData("src/WebUI/Server/DependencyInjection.cs", "services.AddMudServices();")]
    [InlineData("src/WebUI/Client/Program.cs", "builder.Services.AddMudServices();")]
    public void BlazorHosts_Should_RegisterMudServices(string hostPath, string registration)
    {
        var host = Read(hostPath);

        host.ShouldContain("using MudBlazor.Services;");
        host.ShouldContain(registration);
    }

    [Fact]
    public void ClientHost_Should_UseBearerTokensForProtectedApis()
    {
        var program = Read("src/WebUI/Client/Program.cs");

        program.ShouldContain("builder.Services.AddTransient<BearerTokenHandler>();");
        program.ShouldNotContain("builder.Services.AddTransient<CookieCredentialsHandler>();");
        program.ShouldContain("builder.Services.AddScoped<IAuthService, AuthService>();");
        program.ShouldContain("builder.Services.AddScoped<AuthStateService>();");
        program.ShouldContain("builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();");
        program.ShouldContain("AddRefitClient<IUserApi>()");
        program.ShouldContain("AddHttpMessageHandler<BearerTokenHandler>()");
    }

    [Fact]
    public void ServerHost_Should_ForwardBearerTokensThroughBffProxy()
    {
        var program = Read("src/WebUI/Server/Program.cs");
        var services = Read("src/WebUI/Server/DependencyInjection.cs");
        var proxy = Read("src/WebUI/Server/Extentions/BffProxyExtentions.cs");

        program.ShouldContain("app.MapBffProxy();");
        services.ShouldContain("services.AddHttpClient(\"BFFProxy\");");
        services.ShouldContain("configuration[\"ApiBaseUrl\"] ?? \"http://localhost:15000\"");
        proxy.ShouldContain("app.Map(\"/api/{**proxyPath}\"");
        proxy.ShouldContain("$\"/api/{proxyPath}\"");
        proxy.ShouldNotContain("AttachBearerTokenFromCookie");
        proxy.ShouldNotContain("TrySetAuthCookies");
        proxy.ShouldNotContain("HttpOnly = true");
        proxy.ShouldContain("Authorization");
    }

    [Fact]
    public void AuthEffects_Should_UseAuthStateServiceWithoutPermissions()
    {
        var source = Read("src/WebUI/Shared/Store/Auth/AuthEffects.cs");

        source.ShouldContain("AuthStateService");
        source.ShouldNotContain("IPermissionApi");
        source.ShouldNotContain("GetMyPermissionsAsync");
    }

    [Theory]
    [InlineData("Login.razor")]
    [InlineData("Register.razor")]
    public void LoginAndRegister_Should_RunInWebAssembly_ForLocalStorageAuth(string page)
    {
        var source = Read(Path.Combine("src", "WebUI", "Shared", "Pages", "Auth", page));

        source.ShouldContain("@rendermode InteractiveWebAssembly");
    }

    [Fact]
    public void ServerApp_Should_LoadMudBlazorAssetsAndProviders()
    {
        var app = Read("src/WebUI/Server/Components/App.razor");

        app.ShouldContain("_content/MudBlazor/MudBlazor.min.css");
        app.ShouldContain("_content/MudBlazor/MudBlazor.min.js");
        app.ShouldContain("<MudAppProviders");
    }

    private static string Read(string relativePath)
    {
        return File.ReadAllText(Path.Combine(RepoRoot.FullName, relativePath));
    }

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
