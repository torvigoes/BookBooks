using BookBooks.Domain.Enums;

namespace BookBooks.Domain.Entities;

/// <summary>
/// Represents a user-curated list of books.
/// </summary>
public class BookList
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ListVisibility Visibility { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AppUser? User { get; private set; }

    public IReadOnlyCollection<BookListItem> Items => _items.AsReadOnly();
    private readonly List<BookListItem> _items = new();

    protected BookList() { }

    public BookList(string userId, string name, string? description, ListVisibility visibility)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Visibility = visibility;
        CreatedAt = DateTime.UtcNow;
    }
}
