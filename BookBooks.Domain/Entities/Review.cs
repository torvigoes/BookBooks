namespace BookBooks.Domain.Entities;

/// <summary>
/// Represents a user's review for a specific book.
/// </summary>
public class Review
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string BookId { get; private set; }
    public string UserId { get; private set; }
    public int Rating { get; private set; }
    public string Content { get; private set; }
    public bool ContainsSpoiler { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Book? Book { get; private set; }
    public AppUser? User { get; private set; }

    protected Review() { }

    public Review(string bookId, string userId, int rating, string content, bool containsSpoiler)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        if (content.Length is < 10 or > 5000)
            throw new ArgumentException("Content must be between 10 and 5000 characters.", nameof(content));

        BookId = bookId;
        UserId = userId;
        Rating = rating;
        Content = content;
        ContainsSpoiler = containsSpoiler;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int rating, string content, bool containsSpoiler)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        if (content.Length is < 10 or > 5000)
            throw new ArgumentException("Content must be between 10 and 5000 characters.", nameof(content));

        Rating = rating;
        Content = content;
        ContainsSpoiler = containsSpoiler;
    }
}
