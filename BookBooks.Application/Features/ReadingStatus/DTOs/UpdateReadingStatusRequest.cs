using BookBooks.Domain.Enums;

namespace BookBooks.Application.Features.ReadingStatus.DTOs;

public sealed record UpdateReadingStatusRequest(
    ReadingStatusType Status
);
