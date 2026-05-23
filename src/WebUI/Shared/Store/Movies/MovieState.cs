using WebUI.Shared.Models.Movies;

namespace WebUI.Shared.Store.Movies;

public record MovieState(
    bool IsLoading,
    string? Error,
    List<MovieDto> Movies,
    MovieDto? SelectedMovie);

public record LoadMoviesAction();
public record LoadMoviesSuccessAction(List<MovieDto> Movies);
public record LoadMoviesFailureAction(string Error);

public record LoadMovieByIdAction(int Id);
public record LoadMovieByIdSuccessAction(MovieDto Movie);
public record LoadMovieByIdFailureAction(string Error);
