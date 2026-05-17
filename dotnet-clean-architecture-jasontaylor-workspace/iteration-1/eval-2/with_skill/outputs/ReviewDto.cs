namespace backend.Application.Reviews.Queries.GetReviewsForMovie;

public record ReviewDto(string Title, string Content, int Rating, string UserName);
