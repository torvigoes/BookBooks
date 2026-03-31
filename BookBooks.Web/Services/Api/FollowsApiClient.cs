using System.Net.Http.Json;
using BookBooks.Web.Models.Follows;

namespace BookBooks.Web.Services.Api;

public sealed class FollowsApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public FollowsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<FollowedUserDto>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.GetAsync("/api/follows/me", cancellationToken),
            cancellationToken);

        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<FollowedUserDto>>(cancellationToken))
               ?? [];
    }

    public async Task FollowAsync(string followedUserId, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.PostAsync($"/api/follows/{followedUserId}", null, cancellationToken),
            cancellationToken);

        await EnsureSuccessAsync(response);
    }

    public async Task UnfollowAsync(string followedUserId, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.DeleteAsync($"/api/follows/{followedUserId}", cancellationToken),
            cancellationToken);

        await EnsureSuccessAsync(response);
    }
}
