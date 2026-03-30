using Microsoft.AspNetCore.Identity;

namespace BookBooks.Domain.Entities;

/// <summary>
/// Represents a user in the system, extending the default Identity user.
/// </summary>
public class AppUser : IdentityUser
{
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    private readonly List<Review> _reviews = new();

    public IReadOnlyCollection<BookList> BookLists => _bookLists.AsReadOnly();
    private readonly List<BookList> _bookLists = new();

    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    protected AppUser() { }

    public AppUser(string userName, string email)
    {
        UserName = userName;
        Email = email;
    }

    public void UpdateProfile(string? bio, string? avatarUrl)
    {
        Bio = bio;
        AvatarUrl = avatarUrl;
    }
}
