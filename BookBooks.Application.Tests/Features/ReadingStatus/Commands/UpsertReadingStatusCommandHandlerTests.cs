using BookBooks.Application.Features.ReadingStatus.Commands;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using Moq;
using DomainReadingStatus = BookBooks.Domain.Entities.ReadingStatus;

namespace BookBooks.Application.Tests.Features.ReadingStatus.Commands;

public sealed class UpsertReadingStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBookDoesNotExist()
    {
        var bookRepository = new Mock<IBookRepository>();
        var readingStatusRepository = new Mock<IReadingStatusRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        bookRepository
            .Setup(x => x.GetByIdAsync("book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var handler = new UpsertReadingStatusCommandHandler(
            bookRepository.Object,
            readingStatusRepository.Object,
            unitOfWork.Object);

        var command = new UpsertReadingStatusCommand("book-1", "user-1", ReadingStatusType.Read);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Book not found.", result.Error);
        readingStatusRepository.Verify(x => x.AddAsync(It.IsAny<DomainReadingStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        readingStatusRepository.Verify(x => x.Update(It.IsAny<DomainReadingStatus>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateReadingStatus_WhenNoneExists()
    {
        var bookRepository = new Mock<IBookRepository>();
        var readingStatusRepository = new Mock<IReadingStatusRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        bookRepository
            .Setup(x => x.GetByIdAsync("book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book("Book", "Author", "1234567890", 2024));

        readingStatusRepository
            .Setup(x => x.GetByUserAndBookAsync("user-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainReadingStatus?)null);

        readingStatusRepository
            .Setup(x => x.AddAsync(It.IsAny<DomainReadingStatus>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpsertReadingStatusCommandHandler(
            bookRepository.Object,
            readingStatusRepository.Object,
            unitOfWork.Object);

        var command = new UpsertReadingStatusCommand("book-1", "user-1", ReadingStatusType.WantToRead);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("book-1", result.Value!.BookId);
        Assert.Equal("user-1", result.Value.UserId);
        Assert.Equal(ReadingStatusType.WantToRead, result.Value.Status);
        readingStatusRepository.Verify(x => x.AddAsync(It.IsAny<DomainReadingStatus>(), It.IsAny<CancellationToken>()), Times.Once);
        readingStatusRepository.Verify(x => x.Update(It.IsAny<DomainReadingStatus>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateReadingStatus_WhenItAlreadyExists()
    {
        var bookRepository = new Mock<IBookRepository>();
        var readingStatusRepository = new Mock<IReadingStatusRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var existing = new DomainReadingStatus("user-1", "book-1", ReadingStatusType.WantToRead);

        bookRepository
            .Setup(x => x.GetByIdAsync("book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book("Book", "Author", "1234567890", 2024));

        readingStatusRepository
            .Setup(x => x.GetByUserAndBookAsync("user-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpsertReadingStatusCommandHandler(
            bookRepository.Object,
            readingStatusRepository.Object,
            unitOfWork.Object);

        var command = new UpsertReadingStatusCommand("book-1", "user-1", ReadingStatusType.Read);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(ReadingStatusType.Read, result.Value!.Status);
        readingStatusRepository.Verify(x => x.AddAsync(It.IsAny<DomainReadingStatus>(), It.IsAny<CancellationToken>()), Times.Never);
        readingStatusRepository.Verify(x => x.Update(It.Is<DomainReadingStatus>(r => r == existing)), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
