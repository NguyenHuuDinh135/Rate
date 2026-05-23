using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Movies.Commands.CreateMovie;

public sealed record CreateMovieCommand : IRequest<Result<int>>
{
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public int Year { get; init; }
    public decimal? Rating { get; init; }
    public string TrailerUrl { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public MovieType MovieType { get; init; }
}

public sealed class CreateMovieCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<CreateMovieCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateMovieCommand request, CancellationToken ct)
    {
        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Summary = request.Summary ?? string.Empty,
            Year = request.Year,
            Rating = request.Rating,
            TrailerUrl = request.TrailerUrl ?? string.Empty,
            PosterUrl = request.PosterUrl ?? string.Empty,
            MovieType = request.MovieType
        };

        db.Movies.Add(movie);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("movies:all", ct);
        return Result<int>.Success(movie.Id);
    }
}
