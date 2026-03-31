using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IBookListRepository
{
    Task<BookList?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookList>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default);
    Task<BookListItem?> GetItemAsync(string listId, string bookId, CancellationToken cancellationToken = default);
    Task<int> CountItemsAsync(string listId, CancellationToken cancellationToken = default);
    Task AddAsync(BookList bookList, CancellationToken cancellationToken = default);
    Task AddItemAsync(BookListItem item, CancellationToken cancellationToken = default);
    void Update(BookList bookList);
    void RemoveItem(BookListItem item);
    void Delete(BookList bookList);
}
