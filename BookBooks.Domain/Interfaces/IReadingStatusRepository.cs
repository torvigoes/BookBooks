using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

public interface IReadingStatusRepository
{
    Task<ReadingStatus?> GetByUserAndBookAsync(
        string userId,
        string bookId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ReadingStatus readingStatus, CancellationToken cancellationToken = default);

    void Update(ReadingStatus readingStatus);
}
