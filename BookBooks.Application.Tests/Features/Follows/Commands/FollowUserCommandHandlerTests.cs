using BookBooks.Application.Features.Follows.Commands;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Follows.Commands;

public sealed class FollowUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToFollowYourself()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new FollowUserCommandHandler(userRepository.Object, unitOfWork.Object);

        var result = await handler.Handle(new FollowUserCommand("user-1", "user-1"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("A user cannot follow themselves.", result.Error);
        userRepository.Verify(x => x.AddFollowAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTargetUserDoesNotExist()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(x => x.GetByIdAsync("user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);

        var handler = new FollowUserCommandHandler(userRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new FollowUserCommand("user-1", "user-2"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User to follow was not found.", result.Error);
        userRepository.Verify(x => x.AddFollowAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPersistFollow_WhenRequestIsValid()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository
            .Setup(x => x.GetByIdAsync("user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUser("target", "target@test.com"));

        userRepository
            .Setup(x => x.GetFollowAsync("user-1", "user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserFollow?)null);

        userRepository
            .Setup(x => x.AddFollowAsync(It.IsAny<UserFollow>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new FollowUserCommandHandler(userRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new FollowUserCommand("user-1", "user-2"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        userRepository.Verify(x => x.AddFollowAsync(
            It.Is<UserFollow>(f => f.FollowerId == "user-1" && f.FollowedId == "user-2"),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
