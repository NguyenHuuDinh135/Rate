using Refit;
using WebUI.Shared.Models.Movies;

namespace WebUI.Shared.Services.Api;

public interface IPersonApi
{
    [Get("/api/persons/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<PersonDto>>> GetAllAsync();

    [Get("/api/persons/id/{id}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<PersonDto>> GetByIdAsync(int id);

    [Get("/api/persons/movies/{movieId}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<PersonDto>>> GetByMovieAsync(int movieId);

    [Post("/api/persons/create")]
    Task<int> CreateAsync([Body] PersonDto person);

    [Put("/api/persons/update")]
    Task UpdateAsync([Body] PersonDto person);

    [Delete("/api/persons/delete/{id}")]
    Task DeleteAsync(int id);
}
