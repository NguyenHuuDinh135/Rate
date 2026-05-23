using Refit;
using WebFrontend.Shared.Models.Movies;

namespace WebFrontend.Shared.Services.Api;

public interface IPersonApi
{
    [Get("/api/persons/all")]
    Task<List<PersonDto>> GetAllAsync();

    [Get("/api/persons/id/{id}")]
    Task<PersonDto> GetByIdAsync(int id);

    [Get("/api/persons/movies/{movieId}")]
    Task<List<PersonDto>> GetByMovieAsync(int movieId);

    [Post("/api/persons/create")]
    Task<int> CreateAsync([Body] PersonDto person);

    [Put("/api/persons/update")]
    Task UpdateAsync([Body] PersonDto person);

    [Delete("/api/persons/delete/{id}")]
    Task DeleteAsync(int id);
}
