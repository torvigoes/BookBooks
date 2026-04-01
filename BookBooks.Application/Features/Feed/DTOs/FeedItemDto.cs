namespace BookBooks.Application.Features.Feed.DTOs;

/// <summary>
/// Represents a single review item in the authenticated user's feed timeline.
/// </summary>
public sealed record FeedItemDto(
    string ReviewId,
    string BookId,
    string BookTitle,
    string BookAuthor,
    string? BookCoverImageUrl,
    string UserId,
    string UserDisplayName,
    int Rating,
    string Content,
    bool ContainsSpoiler,
    DateTime CreatedAt
);
