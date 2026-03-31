namespace BookBooks.Domain.Entities;

/// <summary>
/// Represents a follower relationship between two users.
/// </summary>
public class UserFollow
{
    public string FollowerId { get; private set; }
    public string FollowedId { get; private set; }
    public DateTime FollowedAt { get; private set; }

    public AppUser? Follower { get; private set; }
    public AppUser? Followed { get; private set; }

    /// <summary>
    /// Protected constructor for EF Core use only.
    /// Properties will be hydrated via column mapping.
    /// </summary>
    protected UserFollow()
    {
        FollowerId = null!;
        FollowedId = null!;
    }

    public UserFollow(string followerId, string followedId)
    {
        if (followerId == followedId)
            throw new InvalidOperationException("A user cannot follow themselves.");

        FollowerId = followerId;
        FollowedId = followedId;
        FollowedAt = DateTime.UtcNow;
    }
}
