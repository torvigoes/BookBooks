using BookBooks.Application.Features.Lists.Commands;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Lists.Commands;

public sealed class AddBookToListCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenListDoesNotExist()
    {
        var bookListRepository = new Mock<IBookListRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        bookListRepository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookList?)null);

        var handler = new AddBookToListCommandHandler(bookListRepository.Object, bookRepository.Object, unitOfWork.Object);
        var command = new AddBookToListCommand("list-1", "user-1", "book-1", "note");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("List not found.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenListBelongsToAnotherUser()
    {
        var bookListRepository = new Mock<IBookListRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var list = new BookList("other-user", "Favorites", null, ListVisibility.Public);

        bookListRepository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new AddBookToListCommandHandler(bookListRepository.Object, bookRepository.Object, unitOfWork.Object);
        var command = new AddBookToListCommand("list-1", "user-1", "book-1", "note");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Forbidden: you can only change your own lists.", result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBookIsAlreadyInList()
    {
        var bookListRepository = new Mock<IBookListRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var list = new BookList("user-1", "Favorites", null, ListVisibility.Public);
        var existingItem = new BookListItem("list-1", "book-1", 1, null);

        bookListRepository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        bookRepository
            .Setup(x => x.GetByIdAsync("book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book("Book", "Author", "1234567890", 2024));

        bookListRepository
            .Setup(x => x.GetItemAsync("list-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var handler = new AddBookToListCommandHandler(bookListRepository.Object, bookRepository.Object, unitOfWork.Object);
        var command = new AddBookToListCommand("list-1", "user-1", "book-1", "note");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("This book is already in the list.", result.Error);
        bookListRepository.Verify(x => x.AddItemAsync(It.IsAny<BookListItem>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAddBookWithNextOrder_WhenRequestIsValid()
    {
        var bookListRepository = new Mock<IBookListRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var list = new BookList("user-1", "Favorites", null, ListVisibility.Public);

        bookListRepository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        bookRepository
            .Setup(x => x.GetByIdAsync("book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book("Book", "Author", "1234567890", 2024));

        bookListRepository
            .Setup(x => x.GetItemAsync("list-1", "book-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookListItem?)null);

        bookListRepository
            .Setup(x => x.CountItemsAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        bookListRepository
            .Setup(x => x.AddItemAsync(It.IsAny<BookListItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bookListRepository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new AddBookToListCommandHandler(bookListRepository.Object, bookRepository.Object, unitOfWork.Object);
        var command = new AddBookToListCommand("list-1", "user-1", "book-1", "top priority");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        bookListRepository.Verify(
            x => x.AddItemAsync(
                It.Is<BookListItem>(item =>
                    item.BookListId == "list-1" &&
                    item.BookId == "book-1" &&
                    item.Order == 4 &&
                    item.Notes == "top priority"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
