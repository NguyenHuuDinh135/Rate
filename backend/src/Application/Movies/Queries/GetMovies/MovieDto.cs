namespace backend.Application.Movies.Queries.GetMovies;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int Year { get; set; }
    public decimal? Rating { get; set; }
}