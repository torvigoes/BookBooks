namespace BookBooks.Application.Features.Reviews.DTOs;

/// <summary>
/// Represents a review returned by the API.
/// </summary>
public record ReviewDto(
    string Id,
    string BookId,
    string UserId,
    string UserDisplayName,
    int Rating,
    string Content,
    bool ContainsSpoiler,
    DateTime CreatedAt
);