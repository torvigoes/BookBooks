using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByBookIdAsync(string bookId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Include(r => r.Book)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetFeedByFollowerIdAsync(string followerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var followedUserIds = _context.UserFollows
            .Where(x => x.FollowerId == followerId)
            .Select(x => x.FollowedId);

        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .Where(r => followedUserIds.Contains(r.UserId))
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<Review?> GetUserReviewForBookAsync(string userId, string bookId, CancellationToken cancellationToken = default)
    {
        return _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId, cancellationToken);
    }

    public async Task<(double AverageRating, int ReviewCount)> GetBookRatingStatsAsync(string bookId, CancellationToken cancellationToken = default)
    {
        var query = _context.Reviews.Where(r => r.BookId == bookId);
        var reviewCount = await query.CountAsync(cancellationToken);

        if (reviewCount == 0)
        {
            return (0, 0);
        }

        var averageRating = await query.AverageAsync(r => (double)r.Rating, cancellationToken);
        return (averageRating, reviewCount);
    }

    public Task<ReviewLike?> GetReviewLikeAsync(string reviewId, string userId, CancellationToken cancellationToken = default)
    {
        return _context.ReviewLikes
            .FirstOrDefaultAsync(x => x.ReviewId == reviewId && x.UserId == userId, cancellationToken);
    }

    public Task<int> GetReviewLikesCountAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        return _context.ReviewLikes.CountAsync(x => x.ReviewId == reviewId, cancellationToken);
    }

    public async Task AddLikeAsync(ReviewLike reviewLike, CancellationToken cancellationToken = default)
    {
        await _context.ReviewLikes.AddAsync(reviewLike, cancellationToken);
    }

    public void RemoveLike(ReviewLike reviewLike)
    {
        _context.ReviewLikes.Remove(reviewLike);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
    }

    public void Update(Review review)
    {
        _context.Reviews.Update(review);
    }

    public void Delete(Review review)
    {
        _context.Reviews.Remove(review);
    }
}
