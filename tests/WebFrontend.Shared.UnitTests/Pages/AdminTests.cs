using Microsoft.AspNetCore.Components;
using Xunit;
using WebFrontend.Shared.Pages.Admin;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using WebFrontend.Shared.Store.Auth;
using WebFrontend.Shared.Store.Movies;
using WebFrontend.Shared.Models.Movies;
using WebFrontend.Shared.Services.Api;
using Moq;
using Fluxor;

namespace WebFrontend.Shared.UnitTests.Pages;

public class AdminTests : BunitContext
{
    public AdminTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
        
        var mockAuth = new Mock<IState<AuthState>>();
        mockAuth.Setup(s => s.Value).Returns(new AuthState(true, false, null, null, new List<string> { "Permission.ManagePermissions" }));
        Services.AddSingleton(mockAuth.Object);

        var mockMovie = new Mock<IState<MovieState>>();
        mockMovie.Setup(s => s.Value).Returns(new MovieState(false, null, new List<MovieDto> {
            new MovieDto(1, "Movie 1", "Summary", 2024, 8.5m, "", "", Models.Common.MovieType.NowShowing, new List<GenreDto>())
        }, null));
        Services.AddSingleton(mockMovie.Object);

        var mockDispatcher = new Mock<IDispatcher>();
        Services.AddSingleton(mockDispatcher.Object);

        // API Mocks
        Services.AddSingleton(new Mock<IMovieApi>().Object);
        Services.AddSingleton(new Mock<IPaymentApi>().Object);
        Services.AddSingleton(new Mock<IShowApi>().Object);
        Services.AddSingleton(new Mock<IBookingApi>().Object);
    }

    [Fact]
    public void AdminMovies_Should_RenderHeader()
    {
        var cut = Render<Movies>();
        cut.Find("h1").TextContent.ShouldContain("Quản lý phim");
    }

    [Fact]
    public void Dashboard_Should_RenderStatCards()
    {
        var cut = Render<Dashboard>();
        cut.FindAll(".grid").ShouldNotBeEmpty();
    }
}
