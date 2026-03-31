using BookBooks.Application.Features.Books.Queries;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Books.Queries;

public sealed class SearchBooksQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldForwardSearchArgumentsAndMapDtos()
    {
        var repository = new Mock<IBookRepository>();

        var books = new List<Book>
        {
            new("Domain-Driven Design", "Eric Evans", "9780321125217", 2003),
            new("Clean Architecture", "Robert C. Martin", "9780134494166", 2017)
        };

        repository
            .Setup(x => x.SearchAsync("architecture", 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var handler = new SearchBooksQueryHandler(repository.Object);
        var query = new SearchBooksQuery("architecture", 2, 5);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, x => x.Title == "Domain-Driven Design" && x.Author == "Eric Evans");
        Assert.Contains(result.Value, x => x.Title == "Clean Architecture" && x.Author == "Robert C. Martin");
        repository.Verify(x => x.SearchAsync("architecture", 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
