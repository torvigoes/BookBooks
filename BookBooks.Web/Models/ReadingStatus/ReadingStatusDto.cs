namespace BookBooks.Web.Models.ReadingStatus;

public sealed record ReadingStatusDto(
    string BookId,
    string UserId,
    ReadingStatusType Status,
    DateTime UpdatedAt
);
