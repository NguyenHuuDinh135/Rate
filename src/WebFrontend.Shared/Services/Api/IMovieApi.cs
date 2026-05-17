using Refit;
using WebFrontend.Shared.Models.Movies;
using WebFrontend.Shared.Models.Common;

namespace WebFrontend.Shared.Services.Api;

public interface IMovieApi
{
    [Get("/api/movies/all")]
    Task<List<MovieDto>> GetAllAsync();

    [Get("/api/movies/id/{id}")]
    Task<MovieDto> GetByIdAsync(int id);

    [Get("/api/movies/filtered")]
    Task<List<MovieDto>> GetFilteredAsync(string? title, MovieType? type, int? year);

    [Get("/api/movies/id/{id}/persons")]
    Task<List<PersonDto>> GetPersonsAsync(int id);
}

public interface IGenreApi
{
    [Get("/api/genre/all")]
    Task<List<GenreDto>> GetAllAsync();

    [Get("/api/genre/id/{id}")]
    Task<GenreDto> GetByIdAsync(int id);

    [Get("/api/genre/movies/{movieId}")]
    Task<List<GenreDto>> GetByMovieAsync(int movieId);
}
