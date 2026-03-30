using BookBooks.Domain.Enums;

namespace BookBooks.Domain.Entities;

/// <summary>
/// Tracks a user's reading status for a specific book.
/// </summary>
public class ReadingStatus
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string UserId { get; private set; }
    public string BookId { get; private set; }
    public ReadingStatusType Status { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public AppUser? User { get; private set; }
    public Book? Book { get; private set; }

    protected ReadingStatus() { }

    public ReadingStatus(string userId, string bookId, ReadingStatusType status)
    {
        UserId = userId;
        BookId = bookId;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ReadingStatusType newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
