namespace BookBooks.Application.Features.Reviews.DTOs;

/// <summary>
/// Request payload to update an existing review.
/// </summary>
public record UpdateReviewRequest(
    int Rating,
    string Content,
    bool ContainsSpoiler
);