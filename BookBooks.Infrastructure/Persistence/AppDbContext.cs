using BookBooks.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookBooks.Infrastructure.Persistence;

/// <summary>
/// Main application database context extending Identity for AppUser.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewLike> ReviewLikes => Set<ReviewLike>();
    public DbSet<BookList> BookLists => Set<BookList>();
    public DbSet<BookListItem> BookListItems => Set<BookListItem>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    public DbSet<ReadingStatus> ReadingStatuses => Set<ReadingStatus>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Scans the assembly for all IEntityTypeConfiguration classes
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
