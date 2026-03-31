using BookBooks.Domain.Enums;

namespace BookBooks.Application.Features.ReadingStatus.DTOs;

public sealed record ReadingStatusDto(
    string BookId,
    string UserId,
    ReadingStatusType Status,
    DateTime UpdatedAt
);
