using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByBookIdAsync(string bookId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Review?> GetUserReviewForBookAsync(string userId, string bookId, CancellationToken cancellationToken = default);
    Task<(double AverageRating, int ReviewCount)> GetBookRatingStatsAsync(string bookId, CancellationToken cancellationToken = default);
    Task<ReviewLike?> GetReviewLikeAsync(string reviewId, string userId, CancellationToken cancellationToken = default);
    Task<int> GetReviewLikesCountAsync(string reviewId, CancellationToken cancellationToken = default);
    Task AddLikeAsync(ReviewLike reviewLike, CancellationToken cancellationToken = default);
    void RemoveLike(ReviewLike reviewLike);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);
    void Delete(Review review);
}
