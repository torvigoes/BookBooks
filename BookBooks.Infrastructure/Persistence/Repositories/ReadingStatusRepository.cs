using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence.Repositories;

public sealed class ReadingStatusRepository : IReadingStatusRepository
{
    private readonly AppDbContext _context;

    public ReadingStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ReadingStatus?> GetByUserAndBookAsync(
        string userId,
        string bookId,
        CancellationToken cancellationToken = default)
    {
        return _context.ReadingStatuses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId, cancellationToken);
    }

    public async Task AddAsync(ReadingStatus readingStatus, CancellationToken cancellationToken = default)
    {
        await _context.ReadingStatuses.AddAsync(readingStatus, cancellationToken);
    }

    public void Update(ReadingStatus readingStatus)
    {
        _context.ReadingStatuses.Update(readingStatus);
    }
}
