using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Book?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _context.Books
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return _context.Books
            .FirstOrDefaultAsync(b => b.Isbn == isbn, cancellationToken);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
        }

        return await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await _context.Books.AddAsync(book, cancellationToken);
    }

    public void Update(Book book)
    {
        _context.Books.Update(book);
    }
}
