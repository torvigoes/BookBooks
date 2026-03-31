namespace BookBooks.Web.Models.Books;

/// <summary>
/// Request payload used to create a book.
/// </summary>
public sealed record CreateBookRequest(
    string Title,
    string Author,
    string Isbn,
    int Year,
    string? CoverImageUrl
);
