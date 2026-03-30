using BookBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBooks.Infrastructure.Persistence.Configurations;

// For brevity, we are putting mapped composite keys in one file, 
// but you can separate them into UserFollowConfiguration.cs, etc.

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.HasKey(f => new { f.FollowerId, f.FollowedId });

        // Configure relationships and disable cascade delete to fix multiple paths issue
        builder.HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Followed)
            .WithMany()
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReviewLikeConfiguration : IEntityTypeConfiguration<ReviewLike>
{
    public void Configure(EntityTypeBuilder<ReviewLike> builder)
    {
        builder.HasKey(l => new { l.UserId, l.ReviewId });

        // Configure relationships and disable cascade delete to fix multiple paths issue
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Review)
            .WithMany()
            .HasForeignKey(l => l.ReviewId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookListItemConfiguration : IEntityTypeConfiguration<BookListItem>
{
    public void Configure(EntityTypeBuilder<BookListItem> builder)
    {
        builder.HasKey(i => new { i.BookListId, i.BookId });
    }
}

public class ReadingStatusConfiguration : IEntityTypeConfiguration<ReadingStatus>
{
    public void Configure(EntityTypeBuilder<ReadingStatus> builder)
    {
        // One status per user+book pair
        builder.HasKey(rs => new { rs.UserId, rs.BookId });
    }
}
