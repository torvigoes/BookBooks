namespace BookBooks.Web.Models.Books;

/// <summary>
/// Book representation returned by the API.
/// </summary>
public sealed record BookDto(
    string Id,
    string Title,
    string Author,
    string Isbn,
    int Year,
    string? CoverImageUrl,
    double AverageRating,
    int ReviewCount
);
