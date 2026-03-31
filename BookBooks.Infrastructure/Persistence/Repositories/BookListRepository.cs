using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence.Repositories;

public class BookListRepository : IBookListRepository
{
    private readonly AppDbContext _context;

    public BookListRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<BookList?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _context.BookLists
            .Include(l => l.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<BookList>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.BookLists
            .Include(l => l.Items)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<BookListItem?> GetItemAsync(string listId, string bookId, CancellationToken cancellationToken = default)
    {
        return _context.BookListItems
            .FirstOrDefaultAsync(x => x.BookListId == listId && x.BookId == bookId, cancellationToken);
    }

    public Task<int> CountItemsAsync(string listId, CancellationToken cancellationToken = default)
    {
        return _context.BookListItems.CountAsync(x => x.BookListId == listId, cancellationToken);
    }

    public async Task AddAsync(BookList bookList, CancellationToken cancellationToken = default)
    {
        await _context.BookLists.AddAsync(bookList, cancellationToken);
    }

    public async Task AddItemAsync(BookListItem item, CancellationToken cancellationToken = default)
    {
        await _context.BookListItems.AddAsync(item, cancellationToken);
    }

    public void Update(BookList bookList)
    {
        _context.BookLists.Update(bookList);
    }

    public void RemoveItem(BookListItem item)
    {
        _context.BookListItems.Remove(item);
    }

    public void Delete(BookList bookList)
    {
        _context.BookLists.Remove(bookList);
    }
}
