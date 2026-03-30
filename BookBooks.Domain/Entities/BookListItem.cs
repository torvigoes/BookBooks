namespace BookBooks.Domain.Entities;

/// <summary>
/// Serves as the join entity between BookList and Book, allowing custom ordering and notes.
/// </summary>
public class BookListItem
{
    public string BookListId { get; private set; }
    public string BookId { get; private set; }
    public int Order { get; private set; }
    public string? Notes { get; private set; }

    public BookList? BookList { get; private set; }
    public Book? Book { get; private set; }

    protected BookListItem() { }

    public BookListItem(string bookListId, string bookId, int order, string? notes = null)
    {
        BookListId = bookListId;
        BookId = bookId;
        Order = order;
        Notes = notes;
    }
}
