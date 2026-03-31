namespace BookBooks.Web.Models.Auth;

/// <summary>
/// Register request payload sent to the API.
/// </summary>
public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password
);
