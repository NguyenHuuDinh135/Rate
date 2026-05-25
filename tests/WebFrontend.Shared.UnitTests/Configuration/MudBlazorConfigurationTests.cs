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
    public void ClientHost_Should_RegisterBearerHandlerForProtectedApis()
    {
        var program = Read("src/WebUI/Client/Program.cs");

        program.ShouldContain("builder.Services.AddTransient<BearerTokenHandler>();");
        program.ShouldContain("AddRefitClient<IUserApi>()");
        program.ShouldContain("AddHttpMessageHandler<BearerTokenHandler>()");
    }

    [Fact]
    public void ServerHost_Should_UseBffProxyForProtectedApis()
    {
        var program = Read("src/WebUI/Server/Program.cs");
        var services = Read("src/WebUI/Server/DependencyInjection.cs");
        var proxy = Read("src/WebUI/Server/Extentions/BffProxyExtentions.cs");

        program.ShouldContain("app.MapBffProxy();");
        services.ShouldContain("""services.AddHttpClient("BFFProxy");""");
        proxy.ShouldContain("AttachBearerTokenFromCookie");
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
