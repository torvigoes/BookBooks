namespace BookBooks.Domain.Entities;

/// <summary>
/// Represents a book with bibliographic data and cached rating statistics.
/// </summary>
public class Book
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Isbn { get; private set; }
    public int Year { get; private set; }
    public string? CoverImageUrl { get; private set; }

    // Cached computed fields
    public double AverageRating { get; private set; }
    public int ReviewCount { get; private set; }

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    private readonly List<Review> _reviews = new();

    /// <summary>
    /// Protected constructor for EF Core use only.
    /// Properties will be hydrated via column mapping.
    /// </summary>
    protected Book()
    {
        Title = null!;
        Author = null!;
        Isbn = null!;
    }

    public Book(string title, string author, string isbn, int year, string? coverImageUrl = null)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        Year = year;
        CoverImageUrl = coverImageUrl;
        AverageRating = 0;
        ReviewCount = 0;
    }

    public void UpdateAverageRating(double newAverage, int newCount)
    {
        AverageRating = newAverage;
        ReviewCount = newCount;
    }
}
