namespace BookBooks.Domain.Entities;

/// <summary>
/// Join entity representing a user liking a specific review.
/// </summary>
public class ReviewLike
{
    public string UserId { get; private set; }
    public string ReviewId { get; private set; }
    public DateTime LikedAt { get; private set; }

    public AppUser? User { get; private set; }
    public Review? Review { get; private set; }

    protected ReviewLike() { }

    public ReviewLike(string userId, string reviewId)
    {
        UserId = userId;
        ReviewId = reviewId;
        LikedAt = DateTime.UtcNow;
    }
}
