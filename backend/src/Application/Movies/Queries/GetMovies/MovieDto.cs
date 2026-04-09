namespace backend.Application.Movies.Queries.GetMovies;

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Year { get; set; }
    public decimal Rating { get; set; }
}