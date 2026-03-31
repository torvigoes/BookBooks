using BookBooks.Domain.Enums;

namespace BookBooks.Application.Features.Lists.DTOs;

public sealed record CreateListRequest(
    string Name,
    string? Description,
    ListVisibility Visibility
);
