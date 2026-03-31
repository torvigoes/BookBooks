using BookBooks.Application.Features.ReadingStatus.Queries;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using Moq;
using DomainReadingStatus = BookBooks.Domain.Entities.ReadingStatus;

namespace BookBooks.Application.Tests.Features.ReadingStatus.Queries;

public sealed class GetReadingStatusByBookQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenReadingStatusDoesNotExist()
    {
        var repository = new Mock<IReadingStatusRepository>();
        repository
            .Setup(x => x.GetByUserAndBookAsync("user-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainReadingStatus?)null);

        var handler = new GetReadingStatusByBookQueryHandler(repository.Object);
        var query = new GetReadingStatusByBookQuery("book-1", "user-1");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Reading status not found.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnReadingStatusDto_WhenReadingStatusExists()
    {
        var repository = new Mock<IReadingStatusRepository>();
        var readingStatus = new DomainReadingStatus("user-1", "book-1", ReadingStatusType.CurrentlyReading);

        repository
            .Setup(x => x.GetByUserAndBookAsync("user-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(readingStatus);

        var handler = new GetReadingStatusByBookQueryHandler(repository.Object);
        var query = new GetReadingStatusByBookQuery("book-1", "user-1");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("book-1", result.Value!.BookId);
        Assert.Equal("user-1", result.Value.UserId);
        Assert.Equal(ReadingStatusType.CurrentlyReading, result.Value.Status);
    }
}
