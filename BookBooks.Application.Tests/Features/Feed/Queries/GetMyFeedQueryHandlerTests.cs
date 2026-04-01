using BookBooks.Application.Features.Feed.Queries;
using BookBooks.Domain.Entities;
using BookBooks.Domain.Interfaces;
using Moq;

namespace BookBooks.Application.Tests.Features.Feed.Queries;

public sealed class GetMyFeedQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFeedItems()
    {
        var reviewRepository = new Mock<IReviewRepository>();

        IReadOnlyCollection<Review> reviews =
        [
            new("book-1", "user-2", 5, "Excellent read with great pacing.", false),
            new("book-2", "user-3", 4, "Great ideas and practical insights.", false)
        ];

        reviewRepository
            .Setup(x => x.GetFeedByFollowerIdAsync("user-1", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);

        var handler = new GetMyFeedQueryHandler(reviewRepository.Object);
        var result = await handler.Handle(new GetMyFeedQuery("user-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, x => x.BookId == "book-1" && x.Rating == 5);
        Assert.Contains(result.Value, x => x.BookId == "book-2" && x.Rating == 4);
    }
}
