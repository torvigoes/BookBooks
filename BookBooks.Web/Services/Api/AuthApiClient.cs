using System.Net.Http.Json;
using BookBooks.Web.Models.Auth;

namespace BookBooks.Web.Services.Api;

public sealed class AuthApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request, cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken))
               ?? throw new ApiException("Invalid authentication response payload.");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request, cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken))
               ?? throw new ApiException("Invalid authentication response payload.");
    }
}
