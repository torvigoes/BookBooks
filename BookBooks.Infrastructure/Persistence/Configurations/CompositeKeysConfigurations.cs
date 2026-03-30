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
    }
}

public class ReviewLikeConfiguration : IEntityTypeConfiguration<ReviewLike>
{
    public void Configure(EntityTypeBuilder<ReviewLike> builder)
    {
        builder.HasKey(l => new { l.UserId, l.ReviewId });
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
