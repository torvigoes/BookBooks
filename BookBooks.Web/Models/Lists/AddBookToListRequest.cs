namespace BookBooks.Web.Models.Lists;

public sealed record AddBookToListRequest(
    string BookId,
    string? Notes
);
