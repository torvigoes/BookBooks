using BookBooks.Application.Features.Lists.Queries;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Enums;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Lists.Queries;

public sealed class GetListByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenListIsPrivateAndUserIsNotOwner()
    {
        var repository = new Mock<IBookListRepository>();
        var list = new BookList("owner-1", "Private List", null, ListVisibility.Private);

        repository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new GetListByIdQueryHandler(repository.Object);
        var query = new GetListByIdQuery("list-1", "other-user");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Forbidden: this list is private.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCurrentUserIsOwner()
    {
        var repository = new Mock<IBookListRepository>();
        var list = new BookList("owner-1", "Private List", "my notes", ListVisibility.Private);

        repository
            .Setup(x => x.GetByIdAsync("list-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new GetListByIdQueryHandler(repository.Object);
        var query = new GetListByIdQuery("list-1", "owner-1");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("owner-1", result.Value!.UserId);
        Assert.Equal("Private List", result.Value.Name);
        Assert.Equal(ListVisibility.Private, result.Value.Visibility);
    }
}
