using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Tests;

public sealed class ReviewTests
{
    [Fact]
    public void Constructor_ShouldCreateReview_WhenInputIsValid()
    {
        var review = new Review("book-1", "user-1", 4, "This is a valid review content.", false);

        Assert.Equal("book-1", review.BookId);
        Assert.Equal("user-1", review.UserId);
        Assert.Equal(4, review.Rating);
        Assert.False(review.ContainsSpoiler);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRatingIsOutOfRange()
    {
        var action = () => new Review("book-1", "user-1", 6, "This is a valid review content.", false);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenContentIsTooShort()
    {
        var action = () => new Review("book-1", "user-1", 5, "short", false);

        Assert.Throws<ArgumentException>(action);
    }
}
