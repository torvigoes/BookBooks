namespace BookBooks.Application.Features.Reviews.DTOs;

/// <summary>
/// Response returned when toggling a review like.
/// </summary>
public record ToggleReviewLikeResponse(
    string ReviewId,
    bool LikedByCurrentUser,
    int LikeCount
);