namespace BookBooks.Web.Models.Lists;

public sealed record BookListItemDto(
    string BookId,
    string BookTitle,
    string BookAuthor,
    int Order,
    string? Notes
);
