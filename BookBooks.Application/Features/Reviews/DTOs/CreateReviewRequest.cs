namespace BookBooks.Application.Features.Reviews.DTOs;

/// <summary>
/// Request payload to create a new review for a book.
/// </summary>
public record CreateReviewRequest(
    int Rating,
    string Content,
    bool ContainsSpoiler
);