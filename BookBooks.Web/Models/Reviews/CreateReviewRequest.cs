namespace BookBooks.Web.Models.Reviews;

/// <summary>
/// Request payload used to create a new review.
/// </summary>
public sealed record CreateReviewRequest(
    int Rating,
    string Content,
    bool ContainsSpoiler
);
