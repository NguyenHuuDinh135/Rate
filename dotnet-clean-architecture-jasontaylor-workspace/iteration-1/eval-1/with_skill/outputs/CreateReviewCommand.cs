using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Domain.Entities;

namespace backend.Application.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand : IRequest<Result<int>>
{
    public int MovieId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public int Rating { get; init; }
}

public sealed class CreateReviewCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateReviewCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var review = new Review
        {
            MovieId = request.MovieId,
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            Rating = request.Rating
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync(ct);

        return Result<int>.Success(review.Id);
    }
}
