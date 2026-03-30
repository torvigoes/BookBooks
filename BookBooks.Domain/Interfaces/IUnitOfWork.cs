namespace BookBooks.Domain.Interfaces;

/// <summary>
/// Defines the contract for committing transactions to the database.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
