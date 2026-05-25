using Shouldly;
using Xunit;

namespace WebUI.Shared.UnitTests.Pages;

public class AdminTests
{
    private static readonly DirectoryInfo RepoRoot = FindRepoRoot();

    [Fact]
    public void AdminMovies_Should_DefineMudBlazorHeader()
    {
        var page = Read("src/WebUI/Shared/Pages/Admin/Movies.razor");

        page.ShouldContain("<PageHeader Title=\"Quản lý phim\"");
        page.ShouldContain("<DataTable");
    }

    [Fact]
    public void Dashboard_Should_RenderStatCards()
    {
        var page = Read("src/WebUI/Shared/Pages/Admin/Dashboard.razor");

        page.ShouldContain("rate-stat-grid");
        page.ShouldContain("WebUI.Shared.Components.Admin.StatCard");
    }

    [Fact]
    public void AdminRoutes_Should_KeepRequestedAliases()
    {
        Read("src/WebUI/Shared/Pages/Admin/Dashboard.razor").ShouldContain("@page \"/admin/dashboard\"");
        Read("src/WebUI/Shared/Pages/Admin/Theaters.razor").ShouldContain("@page \"/admin/cinemas\"");
        Read("src/WebUI/Shared/Pages/Admin/Theaters.razor").ShouldContain("@page \"/admin/rooms\"");
        Read("src/WebUI/Shared/Pages/Admin/Shows.razor").ShouldContain("@page \"/admin/showtimes\"");
        Read("src/WebUI/Shared/Pages/Admin/Users.razor").ShouldContain("@page \"/admin/activity\"");
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
