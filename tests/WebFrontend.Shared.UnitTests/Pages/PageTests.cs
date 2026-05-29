using Microsoft.AspNetCore.Components;
using Xunit;
using WebUI.Shared.Pages;
using Microsoft.FluentUI.AspNetCore.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using WebUI.Shared.Store.Auth;
using WebUI.Shared.Store.Movies;
using WebUI.Shared.Services.Api;
using Moq;
using Fluxor;

namespace WebUI.Shared.UnitTests.Pages;

public class PageTests : BunitContext
{
    public PageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
        
        var mockAuth = new Mock<IState<AuthState>>();
        mockAuth.Setup(s => s.Value).Returns(new AuthState(false, false, null, null, new List<string>()));
        Services.AddSingleton(mockAuth.Object);

        var mockMovie = new Mock<IState<MovieState>>();
        mockMovie.Setup(s => s.Value).Returns(new MovieState(false, null, new List<Models.Movies.MovieDto>(), null));
        Services.AddSingleton(mockMovie.Object);

        var mockDispatcher = new Mock<IDispatcher>();
        Services.AddSingleton(mockDispatcher.Object);
        Services.AddSingleton(new Mock<IActionSubscriber>().Object);

        // API Mocks
        Services.AddSingleton(new Mock<IMovieApi>().Object);
        Services.AddSingleton(new Mock<IPaymentApi>().Object);
        Services.AddSingleton(new Mock<IShowApi>().Object);
        Services.AddSingleton(new Mock<ITheaterApi>().Object);
        Services.AddSingleton(new Mock<IBookingApi>().Object);
        Services.AddSingleton(new Mock<IAuthApi>().Object);
        Services.AddSingleton(new Mock<IUserApi>().Object);
        Services.AddSingleton(new Mock<IToastService>().Object);
    }

    [Fact]
    public void Home_Should_DispatchLoadMovies_WhenEmpty()
    {
        var dispatcherMock = new Mock<IDispatcher>();
        Services.AddSingleton(dispatcherMock.Object);

        var cut = Render<Home>();

        dispatcherMock.Verify(d => d.Dispatch(It.IsAny<LoadMoviesAction>()), Times.AtLeastOnce());
    }

    [Fact]
    public void Profile_Should_RenderUserInfo()
    {
        var mockAuth = new Mock<IState<AuthState>>();
        mockAuth.Setup(s => s.Value).Returns(new AuthState(true, false, null, 
            new Models.Auth.TokenResponseDto("tk", null, "dinhnh", "dinh@rate.com", 3600, new List<string>()), 
            new List<string>()));
        Services.AddSingleton(mockAuth.Object);

        var mockUserApi = new Mock<IUserApi>();
        mockUserApi
            .Setup(api => api.GetMeAsync())
            .ReturnsAsync(new Models.Auth.AuthUserDto("user-1", "dinhnh", "dinh@rate.com", new List<string>()));
        Services.AddSingleton(mockUserApi.Object);

        var cut = Render<Profile>();

        cut.Find("h2").TextContent.ShouldContain("dinhnh");
    }
}
