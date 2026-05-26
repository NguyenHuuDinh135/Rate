using Fluxor;
using WebUI.Shared.Services.Api;

namespace WebUI.Shared.Store.Movies;

public class MovieEffects(IMovieApi movieApi)
{
    [EffectMethod]
    public async Task HandleLoadMovies(LoadMoviesAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await movieApi.GetAllAsync();
            dispatcher.Dispatch(new LoadMoviesSuccessAction(response.Body));
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
            var response = await movieApi.GetByIdAsync(action.Id);
            dispatcher.Dispatch(new LoadMovieByIdSuccessAction(response.Body));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadMovieByIdFailureAction(ex.Message));
        }
    }
}
