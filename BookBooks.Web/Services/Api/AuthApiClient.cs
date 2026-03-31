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
        var response = await SendRequestAsync(
            () => _httpClient.PostAsJsonAsync("/api/auth/login", request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken))
               ?? throw new ApiException("Invalid authentication response payload.", 500);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.PostAsJsonAsync("/api/auth/register", request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken))
               ?? throw new ApiException("Invalid authentication response payload.", 500);
    }
}
