using Refit;
using WebUI.Shared.Models.Theaters;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IShowApi
{
    [Get("/api/shows/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<ShowDto>>> GetAllAsync();

    [Get("/api/shows/id/{id}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<ShowDto>> GetByIdAsync(int id);

    [Get("/api/shows/filters")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<ShowDto>>> GetByMovieAsync(int? movieId, DateTime? date = null);

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
    Task<WebUI.Shared.Models.Common.ApiResponse<List<TheaterDto>>> GetAllAsync();

    [Get("/api/theaters/id/{id}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<TheaterDto>> GetByIdAsync(int id);

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
    ShowType Type);

public record UpdateShowCommand(
    int Id,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int MovieId,
    int TheaterId,
    ShowType Type,
    ShowStatus Status);

public record CreateTheaterCommand(string Name, int NumOfRows, int SeatsPerRow, TheaterType Type);

public record UpdateTheaterCommand(int Id, string Name, int NumOfRows, int SeatsPerRow, TheaterType Type);
