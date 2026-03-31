using BookBooks.Application.Features.Follows.Commands;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Follows.Commands;

public sealed class UnfollowUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFollowRelationshipDoesNotExist()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(x => x.GetFollowAsync("user-1", "user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFollow?)null);

        var handler = new UnfollowUserCommandHandler(userRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new UnfollowUserCommand("user-1", "user-2"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Follow relationship not found.", result.Error);
        userRepository.Verify(x => x.RemoveFollow(It.IsAny<UserFollow>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRemoveFollow_WhenRelationshipExists()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var follow = new UserFollow("user-1", "user-2");

        userRepository
            .Setup(x => x.GetFollowAsync("user-1", "user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(follow);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UnfollowUserCommandHandler(userRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new UnfollowUserCommand("user-1", "user-2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        userRepository.Verify(x => x.RemoveFollow(It.Is<UserFollow>(f => f == follow)), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
