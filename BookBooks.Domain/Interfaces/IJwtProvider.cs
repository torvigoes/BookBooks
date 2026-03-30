using BookBooks.Domain.Entities;

namespace BookBooks.Domain.Interfaces;

/// <summary>
/// Abstraction for generating JSON Web Tokens (JWT).
/// </summary>
public interface IJwtProvider
{
    string Generate(AppUser user);
}
