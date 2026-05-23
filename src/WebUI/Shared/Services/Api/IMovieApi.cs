using Refit;
using WebUI.Shared.Models.Movies;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

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

public record CreateGenreCommand(string Name);
public record UpdateGenreCommand(int Id, string Name);

public interface IGenreApi
{
    [Get("/api/genres/all")]
    Task<List<GenreDto>> GetAllAsync();

    [Get("/api/genres/id/{id}")]
    Task<GenreDto> GetByIdAsync(int id);

    [Get("/api/genres/movies/{movieId}")]
    Task<List<GenreDto>> GetByMovieAsync(int movieId);

    [Post("/api/genres/create")]
    Task<int> CreateAsync([Body] CreateGenreCommand payload);

    [Put("/api/genres/update")]
    Task UpdateAsync([Body] UpdateGenreCommand payload);

    [Delete("/api/genres/delete/{id}")]
    Task DeleteAsync(int id);
}
