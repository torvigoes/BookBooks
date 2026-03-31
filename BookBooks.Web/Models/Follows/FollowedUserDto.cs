namespace BookBooks.Web.Models.Follows;

public sealed record FollowedUserDto(
    string UserId,
    string Username,
    DateTime FollowedAt
);
