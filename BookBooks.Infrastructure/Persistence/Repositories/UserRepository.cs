using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<AppUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserFollow>> GetFollowedByUserAsync(string followerId, CancellationToken cancellationToken = default)
    {
        var follows = await _context.UserFollows
            .Include(f => f.Followed)
            .Where(f => f.FollowerId == followerId)
            .OrderByDescending(f => f.FollowedAt)
            .ToListAsync(cancellationToken);

        return follows.AsReadOnly();
    }

    public Task<UserFollow?> GetFollowAsync(string followerId, string followedId, CancellationToken cancellationToken = default)
    {
        return _context.UserFollows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId, cancellationToken);
    }

    public async Task AddFollowAsync(UserFollow follow, CancellationToken cancellationToken = default)
    {
        await _context.UserFollows.AddAsync(follow, cancellationToken);
    }

    public void RemoveFollow(UserFollow follow)
    {
        _context.UserFollows.Remove(follow);
    }
}
