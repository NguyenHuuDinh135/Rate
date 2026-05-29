using Fluxor;
using Microsoft.Extensions.Logging;
using WebUI.Shared.Services.Api;

namespace WebUI.Shared.Store.Movies;

public class MovieEffects(IMovieApi movieApi, ILogger<MovieEffects> logger)
{
    [EffectMethod]
    public async Task HandleLoadMovies(LoadMoviesAction action, IDispatcher dispatcher)
    {
        try
        {
            logger.LogDebug("Loading movies via {ApiMethod}.", nameof(IMovieApi.GetAllAsync));
            var response = await movieApi.GetAllAsync();
            logger.LogDebug("Loaded {MovieCount} movies.", response.Body?.Count ?? 0);
            dispatcher.Dispatch(new LoadMoviesSuccessAction(response.Body ?? new()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load movies.");
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
