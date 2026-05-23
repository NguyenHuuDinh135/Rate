using Fluxor;

namespace WebUI.Shared.Store.Movies;

public static class MovieReducers
{
    [ReducerMethod]
    public static MovieState OnLoadMovies(MovieState state, LoadMoviesAction action)
        => state with { IsLoading = true, Error = null };

    [ReducerMethod]
    public static MovieState OnLoadMoviesSuccess(MovieState state, LoadMoviesSuccessAction action)
        => state with { IsLoading = false, Movies = action.Movies, Error = null };

    [ReducerMethod]
    public static MovieState OnLoadMoviesFailure(MovieState state, LoadMoviesFailureAction action)
        => state with { IsLoading = false, Error = action.Error };

    [ReducerMethod]
    public static MovieState OnLoadMovieById(MovieState state, LoadMovieByIdAction action)
        => state with { IsLoading = true, Error = null, SelectedMovie = null };

    [ReducerMethod]
    public static MovieState OnLoadMovieByIdSuccess(MovieState state, LoadMovieByIdSuccessAction action)
        => state with { IsLoading = false, SelectedMovie = action.Movie, Error = null };

    [ReducerMethod]
    public static MovieState OnLoadMovieByIdFailure(MovieState state, LoadMovieByIdFailureAction action)
        => state with { IsLoading = false, Error = action.Error };
}

public class MovieFeature : Feature<MovieState>
{
    public override string GetName() => "Movies";
    protected override MovieState GetInitialState() => new MovieState(false, null, new List<Models.Movies.MovieDto>(), null);
}
