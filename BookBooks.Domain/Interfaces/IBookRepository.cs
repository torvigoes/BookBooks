using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    void Update(Book book);
}
