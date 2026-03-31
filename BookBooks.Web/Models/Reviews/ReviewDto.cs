namespace BookBooks.Web.Models.Reviews;

/// <summary>
/// Review representation returned by the API.
/// </summary>
public sealed record ReviewDto(
    string Id,
    string BookId,
    string UserId,
    string UserDisplayName,
    int Rating,
    string Content,
    bool ContainsSpoiler,
    DateTime CreatedAt
);
