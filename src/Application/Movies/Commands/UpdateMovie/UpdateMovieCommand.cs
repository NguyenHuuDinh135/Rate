using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Enums;

namespace backend.Application.Movies.Commands.UpdateMovie;

public sealed record UpdateMovieCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public int Year { get; init; }
    public decimal? Rating { get; init; }
    public string TrailerUrl { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public MovieType MovieType { get; init; }
}

public sealed class UpdateMovieCommandHandler(IApplicationDbContext db, ICacheService cache)
    : IRequestHandler<UpdateMovieCommand, Result>
{
    public async Task<Result> Handle(UpdateMovieCommand request, CancellationToken ct)
    {
        var movie = await db.Movies.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (movie is null)
            return Result.Failure(new[] { "Movie not found." });

        movie.Title = request.Title.Trim();
        movie.Summary = request.Summary ?? string.Empty;
        movie.Year = request.Year;
        movie.Rating = request.Rating;
        movie.TrailerUrl = request.TrailerUrl ?? string.Empty;
        movie.PosterUrl = request.PosterUrl ?? string.Empty;
        movie.MovieType = request.MovieType;

        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync("movies:all", ct);
        await cache.RemoveAsync($"movies:{request.Id}", ct);
        return Result.Success();
    }
}
