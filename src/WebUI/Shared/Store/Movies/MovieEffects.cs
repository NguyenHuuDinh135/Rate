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
            System.Console.WriteLine("[MovieEffects] Start loading movies via IMovieApi.GetAllAsync()...");
            var response = await movieApi.GetAllAsync();
            System.Console.WriteLine($"[MovieEffects] Loaded movies successfully. Body Count = {response.Body?.Count ?? 0}");
            dispatcher.Dispatch(new LoadMoviesSuccessAction(response.Body ?? new()));
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"[MovieEffects] Exception caught in HandleLoadMovies: {ex.Message}\n{ex.StackTrace}");
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
