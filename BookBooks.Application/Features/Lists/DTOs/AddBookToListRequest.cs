namespace BookBooks.Application.Features.Lists.DTOs;

public sealed record AddBookToListRequest(
    string BookId,
    string? Notes
);
