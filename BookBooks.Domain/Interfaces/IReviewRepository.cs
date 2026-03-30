using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByBookIdAsync(string bookId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Review?> GetUserReviewForBookAsync(string userId, string bookId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);
    void Delete(Review review);
}
