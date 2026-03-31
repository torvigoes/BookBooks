using BookBooks.Application.Features.Follows.Queries;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Follows.Queries;

public sealed class GetMyFollowingQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFollowedUsers()
    {
        var userRepository = new Mock<IUserRepository>();

        IReadOnlyCollection<UserFollow> follows =
        [
            new UserFollow("user-1", "user-2"),
            new UserFollow("user-1", "user-3")
        ];

        userRepository
            .Setup(x => x.GetFollowedByUserAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(follows);

        var handler = new GetMyFollowingQueryHandler(userRepository.Object);
        var result = await handler.Handle(new GetMyFollowingQuery("user-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, x => x.UserId == "user-2");
        Assert.Contains(result.Value, x => x.UserId == "user-3");
    }
}
