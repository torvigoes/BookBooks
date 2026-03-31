namespace BookBooks.Web.Models.Auth;

/// <summary>
/// Authentication payload returned by the API.
/// </summary>
public sealed record AuthResponse(
    string Id,
    string Username,
    string Email,
    string Token
);
