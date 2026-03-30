namespace BookBooks.Application.Features.Auth.DTOs;

public record AuthResponse(
    string Id,
    string Username,
    string Email,
    string Token
);
