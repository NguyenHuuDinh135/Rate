using Refit;
using WebUI.Shared.Models.Theaters;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IShowApi
{
    [Get("/api/shows/all")]
    Task<List<ShowDto>> GetAllAsync();

    [Get("/api/shows/id/{id}")]
    Task<ShowDto> GetByIdAsync(int id);

    [Get("/api/shows/movies/{movieId}")]
    Task<List<ShowDto>> GetByMovieAsync(int movieId);

    [Post("/api/shows/create")]
    Task<OperationResultDto<int>> CreateAsync([Body] CreateShowCommand payload);

    [Put("/api/shows/update")]
    Task UpdateAsync([Body] UpdateShowCommand payload);

    [Delete("/api/shows/id/{id}")]
    Task DeleteAsync(int id);
}

public interface ITheaterApi
{
    [Get("/api/theaters")]
    Task<List<TheaterDto>> GetAllAsync();

    [Get("/api/theaters/id/{id}")]
    Task<TheaterDto> GetByIdAsync(int id);

    [Post("/api/theaters")]
    Task<OperationResultDto<int>> CreateAsync([Body] CreateTheaterCommand payload);

    [Put("/api/theaters")]
    Task UpdateAsync([Body] UpdateTheaterCommand payload);

    [Delete("/api/theaters/id/{id}")]
    Task DeleteAsync(int id);
}

public record CreateShowCommand(
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int MovieId,
    int TheaterId,
    int Type);

public record UpdateShowCommand(
    int Id,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int MovieId,
    int TheaterId,
    int Type,
    int Status);

public record CreateTheaterCommand(string Name, int NumOfRows, int SeatsPerRow, int Type);

public record UpdateTheaterCommand(int Id, string Name, int NumOfRows, int SeatsPerRow, int Type);
