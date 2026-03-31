namespace BookBooks.Application.Features.Lists.DTOs;

public sealed record BookListItemDto(
    string BookId,
    string BookTitle,
    string BookAuthor,
    int Order,
    string? Notes
);
