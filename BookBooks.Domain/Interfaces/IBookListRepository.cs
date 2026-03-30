using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IBookListRepository
{
    Task<BookList?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookList>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(BookList bookList, CancellationToken cancellationToken = default);
    void Update(BookList bookList);
    void Delete(BookList bookList);
}
