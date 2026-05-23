using Refit;
using WebFrontend.Shared.Models.Theaters;
using WebFrontend.Shared.Models.Common;

namespace WebFrontend.Shared.Services.Api;

public interface IShowApi
{
    [Get("/api/shows/all")]
    Task<List<ShowDto>> GetAllAsync();

    [Get("/api/shows/id/{id}")]
    Task<ShowDto> GetByIdAsync(int id);

    [Get("/api/shows/movies/{movieId}")]
    Task<List<ShowDto>> GetByMovieAsync(int movieId);
}

public interface ITheaterApi
{
    [Get("/api/theaters/all")]
    Task<List<TheaterDto>> GetAllAsync();

    [Get("/api/theaters/id/{id}")]
    Task<TheaterDto> GetByIdAsync(int id);
}
