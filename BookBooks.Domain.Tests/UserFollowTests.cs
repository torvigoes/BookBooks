using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Tests;

public sealed class UserFollowTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenFollowerAndFollowedAreTheSame()
    {
        var action = () => new UserFollow("user-1", "user-1");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Constructor_ShouldCreateRelationship_WhenUsersAreDifferent()
    {
        var relation = new UserFollow("user-1", "user-2");

        Assert.Equal("user-1", relation.FollowerId);
        Assert.Equal("user-2", relation.FollowedId);
    }
}
