namespace BookBooks.Web.Models.Auth;

/// <summary>
/// Login request payload sent to the API.
/// </summary>
public sealed record LoginRequest(
    string Email,
    string Password
);
