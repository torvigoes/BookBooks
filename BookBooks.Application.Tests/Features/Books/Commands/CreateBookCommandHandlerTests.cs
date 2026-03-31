using BookBooks.Application.Features.Books.Commands;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Books.Commands;

public sealed class CreateBookCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIsbnAlreadyExists()
    {
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        bookRepository
            .Setup(x => x.GetByIsbnAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book("Existing", "Author", "1234567890", 2024));

        var handler = new CreateBookCommandHandler(bookRepository.Object, unitOfWork.Object);
        var command = new CreateBookCommand("New", "Author", "1234567890", 2024, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("A book with the specified ISBN already exists.", result.Error);
        bookRepository.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPersistBook_WhenRequestIsValid()
    {
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        bookRepository
            .Setup(x => x.GetByIsbnAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        bookRepository
            .Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateBookCommandHandler(bookRepository.Object, unitOfWork.Object);
        var command = new CreateBookCommand("Clean Architecture", "Author", "1234567890", 2024, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value));
        bookRepository.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
