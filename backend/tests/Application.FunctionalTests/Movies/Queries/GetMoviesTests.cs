using backend.Application.FunctionalTests.Infrastructure;
using backend.Application.Movies.Commands.CreateMovie;
using backend.Application.Movies.Queries.GetMovies;
using backend.Domain.Enums;
using Shouldly;

namespace backend.Application.FunctionalTests.Movies.Queries;

using static TestApp;

public class GetMoviesTests : TestBase
{
    [Test]
    public async Task ShouldReturnAllMovies()
    {
        await RunAsAdministratorAsync();

        await SendAsync(new CreateMovieCommand
        {
            Title = "Movie 1",
            Year = 2021,
            MovieType = MovieType.NowShowing
        });

        await SendAsync(new CreateMovieCommand
        {
            Title = "Movie 2",
            Year = 2022,
            MovieType = MovieType.NowShowing
        });

        var query = new GetMoviesQuery();

        var result = await SendAsync(query);

        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.Any(m => m.Title == "Movie 1").ShouldBeTrue();
        result.Any(m => m.Title == "Movie 2").ShouldBeTrue();
    }
}
