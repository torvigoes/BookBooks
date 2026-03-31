namespace BookBooks.Web.Models.Lists;

public sealed record BookListDto(
    string Id,
    string UserId,
    string Name,
    string? Description,
    ListVisibility Visibility,
    DateTime CreatedAt,
    int ItemCount,
    IReadOnlyCollection<BookListItemDto> Items
);
