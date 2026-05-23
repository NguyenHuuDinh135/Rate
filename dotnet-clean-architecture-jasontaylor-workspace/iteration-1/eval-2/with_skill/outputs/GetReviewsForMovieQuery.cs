using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace backend.Application.Reviews.Queries.GetReviewsForMovie;

public sealed record GetReviewsForMovieQuery(int MovieId) : IRequest<Result<List<ReviewDto>>>;

public sealed class GetReviewsForMovieQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetReviewsForMovieQuery, Result<List<ReviewDto>>>
{
    public async Task<Result<List<ReviewDto>>> Handle(GetReviewsForMovieQuery request, CancellationToken ct)
    {
        var reviews = await db.Reviews
            .AsNoTracking()
            .Where(r => r.MovieId == request.MovieId)
            .Select(r => new ReviewDto(
                r.Title,
                r.Content,
                r.Rating,
                r.UserId))
            .ToListAsync(ct);

        return Result<List<ReviewDto>>.Success(reviews);
    }
}
