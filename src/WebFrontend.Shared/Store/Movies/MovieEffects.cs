using Fluxor;
using WebFrontend.Shared.Services.Api;

namespace WebFrontend.Shared.Store.Movies;

public class MovieEffects(IMovieApi movieApi)
{
    [EffectMethod]
    public async Task HandleLoadMovies(LoadMoviesAction action, IDispatcher dispatcher)
    {
        try
        {
            var movies = await movieApi.GetAllAsync();
            dispatcher.Dispatch(new LoadMoviesSuccessAction(movies));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadMoviesFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleLoadMovieById(LoadMovieByIdAction action, IDispatcher dispatcher)
    {
        try
        {
            var movie = await movieApi.GetByIdAsync(action.Id);
            dispatcher.Dispatch(new LoadMovieByIdSuccessAction(movie));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadMovieByIdFailureAction(ex.Message));
        }
    }
}
