using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Models.Movies;

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public int Year { get; set; }
    public decimal? Rating { get; set; }
    public string TrailerUrl { get; set; } = "";
    public string PosterUrl { get; set; } = "";
    public MovieType MovieType { get; set; }
    public List<GenreDto> Genres { get; set; } = new();

    public MovieDto() { }
    public MovieDto(int id, string title, string summary, int year, decimal? rating, string trailerUrl, string posterUrl, MovieType movieType, List<GenreDto> genres)
    {
        Id = id; Title = title; Summary = summary; Year = year; Rating = rating; TrailerUrl = trailerUrl; PosterUrl = posterUrl; MovieType = movieType; Genres = genres;
    }
}

public record FilteredMoviesDto(List<MovieDto> Movies, int TotalCount);

public class PersonDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public int? Age { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Biography { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? PictureUrl { get; set; }
    public string? Role { get; set; }
    public List<PersonMovieDto>? Movies { get; set; }
}

public record PersonMovieDto(
    int Id,
    string Title,
    string PosterUrl,
    int Year,
    string? Role,
    decimal? Rating,
    string? Summary,
    string? TrailerUrl);

public record PersonsForMovieDto(int MovieId, List<PersonDto> Roles);
