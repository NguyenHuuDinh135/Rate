using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace backend.Application.Reviews.Queries.GetReviewsForMovie;

public record GetReviewsForMovieQuery(int MovieId) : IRequest<Result<IReadOnlyList<ReviewDto>>>;

public class GetReviewsForMovieQueryHandler(IApplicationDbContext db, IIdentityService identityService)
    : IRequestHandler<GetReviewsForMovieQuery, Result<IReadOnlyList<ReviewDto>>>
{
    public async Task<Result<IReadOnlyList<ReviewDto>>> Handle(GetReviewsForMovieQuery request, CancellationToken ct)
    {
        var reviews = await db.Reviews
            .AsNoTracking()
            .Where(x => x.MovieId == request.MovieId)
            .ToListAsync(ct);

        var dtos = new List<ReviewDto>();

        foreach (var review in reviews)
        {
            var userName = await identityService.GetUserNameAsync(review.UserId) ?? "Unknown User";
            dtos.Add(new ReviewDto(review.Title, review.Content, review.Rating, userName));
        }

        return Result<IReadOnlyList<ReviewDto>>.Success(dtos);
    }
}
