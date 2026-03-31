namespace BookBooks.Application.Features.Follows.DTOs;

public sealed record FollowedUserDto(
    string UserId,
    string Username,
    DateTime FollowedAt
);
