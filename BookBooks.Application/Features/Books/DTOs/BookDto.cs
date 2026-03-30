namespace BookBooks.Application.Features.Books.DTOs;

/// <summary>
/// Data Transfer Object representing a single book along with its statistics.
/// </summary>
public record BookDto(
    string Id,
    string Title,
    string Author,
    string Isbn,
    int Year,
    string? CoverImageUrl,
    double AverageRating,
    int ReviewCount
);
