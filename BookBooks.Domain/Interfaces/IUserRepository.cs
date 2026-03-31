using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserFollow>> GetFollowedByUserAsync(string followerId, CancellationToken cancellationToken = default);

    // Follows
    Task<UserFollow?> GetFollowAsync(string followerId, string followedId, CancellationToken cancellationToken = default);
    Task AddFollowAsync(UserFollow follow, CancellationToken cancellationToken = default);
    void RemoveFollow(UserFollow follow);
}
