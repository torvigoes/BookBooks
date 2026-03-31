namespace BookBooks.Web.Models.ReadingStatus;

public sealed record UpdateReadingStatusRequest(
    ReadingStatusType Status
);
